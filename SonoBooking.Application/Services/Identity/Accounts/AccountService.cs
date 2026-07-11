using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Application.Services.Email;
using SonoBooking.Application.Services.BusinessNotification.Chat;
using SonoBooking.Common.Helpers.MediaUploader;
using SonoBooking.Common.Infrastructure.UnitOfWork;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Identity.Accounts
{

    public class AccountService(
                 UserManager<User> userManager,
                 RoleManager<Role> roleManager,
                 SonoBookingDbContext context,
                 UserDataDto auditUser,
                 IConfiguration configuration,
                 IUnitOfWork<User> UnitOfWork,
                 IMapper Mapper,
                 IWebHostEnvironment hostingEnvironment,
                 IHttpContextAccessor httpContextAccessor,
                 IEmailService emailService,
                 IChatRealtimePublisher chatRealtimePublisher) : IAccountService
    {
        private readonly UploaderConfiguration _uploaderConfiguration = new(hostingEnvironment, httpContextAccessor);
        public async Task<IFinalResult> CheckNationalIdExistsAsync(string nationalId, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            if (string.IsNullOrWhiteSpace(nationalId))
                return responseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                    message: "رقم الهوية مطلوب.");

            string documentNumber = nationalId.Trim();

            bool isEmployee = await IsEmployeeNationalIdAsync(documentNumber, cancellationToken);
            string? employeeId = isEmployee
                ? await GetEmployeeIdByNationalIdAsync(documentNumber, cancellationToken)
                : null;
            bool exists = await NationalIdExistsAsync(documentNumber, cancellationToken);

            return responseResult.PostResult(
                result: new CheckNationalIdResultDto
                {
                    Exists = exists,
                    IsEmployee = isEmployee,
                    EmployeeId = employeeId
                },
                status: HttpStatusCode.OK,
                exception: null,
                message: exists ? MessagesConstants.NationalIdExisted : MessagesConstants.Success);
        }

        public async Task<IFinalResult> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            if (string.IsNullOrWhiteSpace(request.NationalId))
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "رقم الهوية مطلوب.");

            string documentNumber = request.NationalId.Trim();

            if (await NationalIdExistsAsync(documentNumber, cancellationToken))
                return responseResult.PostResult(result: null, status: HttpStatusCode.Conflict, exception: null,
                    message: MessagesConstants.NationalIdExisted);

            string? employeeId = await IsEmployeeNationalIdAsync(documentNumber, cancellationToken)
                ? await GetEmployeeIdByNationalIdAsync(documentNumber, cancellationToken)
                : null;

            string? leaderId = string.IsNullOrWhiteSpace(request.LeaderId) ? null : request.LeaderId.Trim();
            if (leaderId != null)
            {
                bool leaderExists = await context.Leaders
                    .AnyAsync(l => l.Id == leaderId && !l.IsDeleted && l.IsActive, cancellationToken);
                if (!leaderExists)
                    return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: "Invalid or inactive leader.");
            }

            User checkUser = await userManager.FindByEmailAsync(request.Email);

            if (checkUser != null)
                return responseResult.PostResult(result: null, status: HttpStatusCode.Conflict, exception: null,
                    message: MessagesConstants.EmailExisted);

            if (request.DocumentImage == null)
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "Document image is required.");

            if (string.IsNullOrWhiteSpace(request.JobTitle))
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "المسمى الوظيفي مطلوب.");

            if (string.IsNullOrWhiteSpace(request.Organization))
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "الجهة مطلوبة.");

            string documentUpload = await _uploaderConfiguration.UploadFile(request.DocumentImage, "Attach/Users", cancellationToken);
            if (UploadResponse(documentUpload) is { } uploadErr)
                return uploadErr;

            User user = new()
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.Username,
                DocumentNumber = documentNumber,
                DocumentType = request.DocumentType,
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                PhoneNumber = request.Phone,
                DocumentImageUrl = documentUpload,
                EmployeeId = employeeId,
                LeaderId = leaderId,
                JobTitle = request.JobTitle.Trim(),
                Organization = request.Organization.Trim(),
                CreatedBy = auditUser.Name != "" ? auditUser.Name : request.Username,
                CreatedById = auditUser.Id != "" ? auditUser.Id : "",
                CreatedAt = DateTime.UtcNow,
                ModifiedBy = auditUser.Name != "" ? auditUser.Name : request.Username,
                ModifiedById = auditUser.Id != "" ? auditUser.Id : "",
                ModifiedAt = DateTime.UtcNow,
            };

            IdentityResult result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return responseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                 message: MessagesConstants.AddError + "User : " +
                                                 string.Join(", ", result.Errors.Select(e => e.Description)));

            user.CreatedById = auditUser.Name != "" ? auditUser.Name : user.FullName;
            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedById = auditUser.Name != "" ? auditUser.Name : user.FullName;
            user.ModifiedAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            if (!string.IsNullOrWhiteSpace(request.RoleId))
            {
                Role role = await roleManager.FindByIdAsync(request.RoleId);

                IdentityResult res = await userManager.AddToRoleAsync(user, role.Name!);

                if (!res.Succeeded)
                    return responseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.AddError + "User : " +
                                                     string.Join(", ", result.Errors.Select(e => e.Description)));

                return responseResult.PostResult(result: user.Id, status: HttpStatusCode.Created, exception: null,
                                                 message: MessagesConstants.AddSuccess);
            }

            Role defaultRole = await roleManager.FindByNameAsync(RoleNames.User);

            IdentityResult defaultRoleResult = await userManager.AddToRoleAsync(user, defaultRole.Name!);

            if (!defaultRoleResult.Succeeded)
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                                                 message: MessagesConstants.AddError + "User : " +
                                                 string.Join(", ", defaultRoleResult.Errors.Select(e => e.Description)));

            return responseResult.PostResult(result: user.Id, status: HttpStatusCode.Created, exception: null,
                                              message: MessagesConstants.AddSuccess);
        }

        private async Task<bool> NationalIdExistsAsync(string documentNumber, CancellationToken cancellationToken = default)
        {
            return await userManager.Users.AnyAsync(
                u => u.DocumentNumber == documentNumber && !u.IsDeleted,
                cancellationToken);
        }

        private async Task<bool> IsEmployeeNationalIdAsync(string documentNumber, CancellationToken cancellationToken = default) =>
            await context.Employees.AnyAsync(
                e => e.NationalId == documentNumber && !e.IsDeleted,
                cancellationToken);

        private async Task<string?> GetEmployeeIdByNationalIdAsync(string documentNumber, CancellationToken cancellationToken = default) =>
            await context.Employees
                .Where(e => e.NationalId == documentNumber && !e.IsDeleted)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<IFinalResult> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            LoginResponseDto response = new();

            User user = await userManager.FindByEmailAsync(request.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
                return responseResult.PostResult(result: response, status: HttpStatusCode.Unauthorized, exception: null,
                                                 message: "Invalid email or password");

            user.IsLogedIn = true;
            user.ModifiedBy = user.FullName;
            user.ModifiedById = user.Id;
            user.ModifiedAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            await chatRealtimePublisher.PublishUserPresenceChangedAsync(user.Id, isOnline: true, cancellationToken);

            response = await CreateTokenResponse(user, cancellationToken);

            return responseResult.PostResult(result: response, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public async Task<IFinalResult> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();
            const string successMessage = "إذا كان الحساب موجوداً، ستصلك كلمة المرور الجديدة على بريدك الإلكتروني.";

            if (string.IsNullOrWhiteSpace(request.Identifier))
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "يجب إدخال البريد الإلكتروني أو اسم المستخدم.");

            string identifier = request.Identifier.Trim();

            if (IsPasswordResetExcludedEmail(identifier))
                return responseResult.PostResult(result: false, status: HttpStatusCode.OK, exception: null,
                    message: MessagesConstants.PasswordResetContactAdmin);

            User? user = await userManager.FindByEmailAsync(identifier)
                ?? await userManager.FindByNameAsync(identifier);

            if (user is null)
            {
                user = await userManager.Users
                    .FirstOrDefaultAsync(u => !u.IsDeleted && u.FullName == identifier, cancellationToken);
            }

            if (user is null || user.IsDeleted || string.IsNullOrWhiteSpace(user.Email))
            {
                return responseResult.PostResult(result: true, status: HttpStatusCode.OK, exception: null,
                    message: successMessage);
            }

            if (IsPasswordResetExcludedEmail(user.Email))
                return responseResult.PostResult(result: false, status: HttpStatusCode.OK, exception: null,
                    message: MessagesConstants.PasswordResetContactAdmin);

            string newPassword = GenerateTemporaryPassword();

            IdentityResult removeResult = await userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: MessagesConstants.UpdateError);

            IdentityResult addResult = await userManager.AddPasswordAsync(user, newPassword);
            if (!addResult.Succeeded)
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: string.Join(", ", addResult.Errors.Select(e => e.Description)));

            List<RefreshToken> oldTokens = await context.RefreshTokens
                .Where(t => t.UserId == user.Id)
                .ToListAsync(cancellationToken);

            if (oldTokens.Count > 0)
            {
                context.RefreshTokens.RemoveRange(oldTokens);
                await context.SaveChangesAsync(cancellationToken);
            }

            user.IsLogedIn = false;
            user.ModifiedAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            await chatRealtimePublisher.PublishUserPresenceChangedAsync(user.Id, isOnline: false, cancellationToken);

            try
            {
                string subject = "إعادة تعيين كلمة المرور - نظام حجز الإسكان";
                string body = $"""
                    <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً {user.FullName}</h2>
                    <p>تم إعادة تعيين كلمة المرور الخاصة بحسابك في نظام حجز الإسكان بمحافظة أسوان.</p>
                    <p><strong>كلمة المرور الجديدة:</strong> {newPassword}</p>
                    <p>يُرجى تسجيل الدخول وتغيير كلمة المرور في أقرب وقت ممكن.</p>
                    </div>
                    """;
                await emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch
            {
                return responseResult.PostResult(result: null, status: HttpStatusCode.InternalServerError, exception: null,
                    message: "تعذر إرسال البريد الإلكتروني. يُرجى المحاولة لاحقاً.");
            }

            return responseResult.PostResult(result: true, status: HttpStatusCode.OK, exception: null,
                message: successMessage);
        }
        public async Task<IFinalResult> LogoutAsync(string id, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            User user = await userManager.FindByIdAsync(id);

            if (user is not null)
            {
                RefreshToken refreshToken = await context.RefreshTokens.Where(rf => rf.UserId == user.Id).FirstOrDefaultAsync(cancellationToken);
                if (refreshToken is not null)
                {
                    await RemoveOldRefreshToken(id, refreshToken!.Token, cancellationToken);
                }

                user.IsLogedIn = false;
                user.ModifiedBy = user.FullName;
                user.ModifiedById = user.Id;
                user.ModifiedAt = DateTime.UtcNow;
                await userManager.UpdateAsync(user);

                await chatRealtimePublisher.PublishUserPresenceChangedAsync(user.Id, isOnline: false, cancellationToken);

                return responseResult.PostResult(result: true, status: HttpStatusCode.OK, exception: null,
                                                 message: MessagesConstants.Success);
            }

            return responseResult.PostResult(result: false, status: HttpStatusCode.Unauthorized, exception: null,
                                                     message: HttpStatusCode.Unauthorized.ToString());

        }
        public async Task<LoginResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken, cancellationToken);
            if (user is null) return null;

            return await CreateTokenResponse(user, cancellationToken);
        }
        public async Task<IFinalResult> UpdateUser(UpdateUserDto updateUser, CancellationToken cancellationToken = default)
        {
            var responseResult = new ResponseResult();

            User user = await userManager.FindByIdAsync(updateUser.Id);

            if (user is null)
                return responseResult.PostResult(result: null, status: HttpStatusCode.NotFound, exception: null,
                    message: MessagesConstants.NotFound);

            user.Email = updateUser.Email;
            user.UserName = updateUser.Email;
            user.FullName = updateUser.UserName;

            if (!string.IsNullOrWhiteSpace(updateUser.LeaderId))
            {
                string leaderId = updateUser.LeaderId.Trim();
                bool leaderExists = await context.Leaders
                    .AnyAsync(l => l.Id == leaderId && !l.IsDeleted && l.IsActive, cancellationToken);
                if (!leaderExists)
                    return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: "Invalid or inactive leader.");

                user.LeaderId = leaderId;
            }
            else
            {
                user.LeaderId = null;
            }

            if (string.IsNullOrWhiteSpace(updateUser.JobTitle))
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "المسمى الوظيفي مطلوب.");

            if (string.IsNullOrWhiteSpace(updateUser.Organization))
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "الجهة مطلوبة.");

            user.JobTitle = updateUser.JobTitle.Trim();
            user.Organization = updateUser.Organization.Trim();

            user.ModifiedById = auditUser.Name != "" ? auditUser.Name : user.FullName;
            user.ModifiedAt = DateTime.UtcNow;

            if (updateUser.NewPassword != "" && updateUser.NewPassword is not null)
            {
                if (auditUser.Role == RoleNames.SuperAdmin)
                {
                    IdentityResult resRemoveOldPassword = await userManager.RemovePasswordAsync(user);

                    if (!resRemoveOldPassword.Succeeded)
                        return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.UpdateError);

                    IdentityResult setNewPassword = await userManager.AddPasswordAsync(user, updateUser.NewPassword);

                    if (!setNewPassword.Succeeded)
                        return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.UpdateError);
                }
                else
                {
                    IdentityResult updatePassword = await userManager.ChangePasswordAsync
                                                    (user, updateUser.OldPassword, updateUser.NewPassword);
                    if (!updatePassword.Succeeded)
                        return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.UpdateError + "Wrong Old Password !");
                }
            }

            if (updateUser.DocumentImage != null)
            {
                string documentUpload = await _uploaderConfiguration.UploadFile(updateUser.DocumentImage, "Attach/Users", cancellationToken);
                if (UploadResponse(documentUpload) is { } uploadErr)
                    return uploadErr;

                _uploaderConfiguration.DeleteFile(user.DocumentImageUrl);
                user.DocumentImageUrl = documentUpload;
            }

            Role role = await roleManager.FindByIdAsync(updateUser.RoleId.ToString());

            if (role != null)
            {
                IList<string> userRole = await userManager.GetRolesAsync(user);

                if (userRole.Count > 0)
                {
                    await userManager.RemoveFromRolesAsync(user, userRole);
                }

                IdentityResult addRoleRes = await userManager.AddToRoleAsync(user, role.Name);

                if (!addRoleRes.Succeeded)
                    return responseResult.PostResult(user, status: HttpStatusCode.BadRequest,
                                message: "Failed to Update User : " +
                                string.Join(", ", addRoleRes.Errors.Select(e => e.Description)));
            }

            IdentityResult res = await userManager.UpdateAsync(user);

            if (!res.Succeeded)
                return responseResult.PostResult(user, status: HttpStatusCode.BadRequest,
                            message: "Failed to Update User : " +
                            string.Join(", ", res.Errors.Select(e => e.Description)));

            return responseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                             message: MessagesConstants.UpdateSuccess);
        }
        public async Task<IFinalResult> GetUserByIdAsync(string Id, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            User user = await userManager.FindByIdAsync(Id);

            if (user is null)
                return responseResult.PostResult(result: null, status: HttpStatusCode.NotFound, exception: null,
                    message: MessagesConstants.NotFound);

            UserDto userDto = new()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.FullName,
                Role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "",
                BirthDate = user.BirthDate ?? default,
                Phone = user.PhoneNumber,
                Gender = user.Gender ?? default,
                DocumentType = user.DocumentType ?? default,
                DocumentNumber = user.DocumentNumber,
                DocumentImageUrl = user.DocumentImageUrl,
                JobTitle = user.JobTitle,
                Organization = user.Organization,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                ModifiedAt = user.ModifiedAt,
                ModifiedBy = user.ModifiedBy
            };

            userDto.RoleId = roleManager.Roles.Where(r => r.Name == userDto.Role)
                                              .Select(r => r.Id).FirstOrDefault() ?? "";

            userDto.LeaderId = user.LeaderId;
            if (!string.IsNullOrWhiteSpace(user.LeaderId))
            {
                userDto.LeaderName = await context.Leaders
                    .Where(l => l.Id == user.LeaderId)
                    .Select(l => l.FullName)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return responseResult.PostResult(result: userDto, status: HttpStatusCode.OK, exception: null,
                                             message: MessagesConstants.Success);
        }
        public async Task<IFinalResult> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            List<User> userList = await userManager.Users.ToListAsync(cancellationToken);

            List<UserDto> users = [];

            foreach (var u in userList)
            {
                IList<string> roles = await userManager.GetRolesAsync(u);

                users.Add(new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.FullName,
                    Role = roles.FirstOrDefault() ?? "",
                    RoleId = "",
                    BirthDate = u.BirthDate ?? default,
                    Gender = u.Gender ?? default,
                    DocumentType = u.DocumentType ?? default,
                    DocumentNumber = u.DocumentNumber,
                    DocumentImageUrl = u.DocumentImageUrl,
                    LeaderId = u.LeaderId,
                    JobTitle = u.JobTitle,
                    Organization = u.Organization,
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    ModifiedAt = u.ModifiedAt,
                    ModifiedBy = u.ModifiedBy
                });
            }

            if (users == null)
                return responseResult.PostResult(result: null, status: HttpStatusCode.NotFound, exception: null,
                                                 message: MessagesConstants.NotFound);

            for (int i = 0; i < users.Count; i++)
            {
                users[i].RoleId = roleManager.Roles.Where(r => r.Name == users[i].Role)
                                                      .Select(r => r.Id).FirstOrDefault() ?? "";
            }

            return responseResult.PostResult(result: users, status: HttpStatusCode.OK, exception: null,
                                             message: MessagesConstants.Success);
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<FilterUserDto> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var (Count, Result) = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(filter.Filter), pageNumber: offset, pageSize: limit, filter.OrderByValue, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(Result);

            for (int i = 0; i < data.ToList().Count; i++)
            {
                var email = data.ToList()[i].Email;
                var user = await userManager.FindByEmailAsync(email);
                IList<string> userRoles = await userManager.GetRolesAsync(user);
                data.ToList()[i].Role = userRoles.FirstOrDefault() ?? "";
                data.ToList()[i].RoleId = roleManager.Roles.Where(r => r.Name == data.ToList()[i].Role)
                                                     .Select(r => r.Id).FirstOrDefault() ?? "";// Default to User role if no roles assigned
            }

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<IFinalResult> DeleteUser(string userId, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            User user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            IdentityResult res = await userManager.DeleteAsync(user);

            if (!res.Succeeded)
                return responseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                                                 message: MessagesConstants.DeleteError + "User : " +
                                                 string.Join(", ", res.Errors.Select(e => e.Description)));

            return responseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                             message: MessagesConstants.DeleteSuccess);
        }
        private async Task<LoginResponseDto> CreateTokenResponse(User user, CancellationToken cancellationToken = default)
        {
            IList<Claim> perUserClaim = await userManager.GetClaimsAsync(user);

            IList<string> userRole = await userManager.GetRolesAsync(user);

            IList<Claim> perRoleClaim = [];

            for (int i = 0; i < userRole.Count; i++)
            {
                Role roleName = await roleManager.FindByNameAsync(userRole[i]);
                perRoleClaim = await roleManager.GetClaimsAsync(roleName!);
            }

            IEnumerable<Claim> perUser = perUserClaim.Union(perRoleClaim);

            return new LoginResponseDto
            {
                IsLogedIn = true,
                AccessToken = await CreateToken(user, perUser, cancellationToken),
                RefreshToken = await GenerateAndSaveRefreshTokensAsync(user, cancellationToken)
            };
        }
        private async Task<User> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
        {
            User user = await userManager.FindByIdAsync(userId);

            var token = await context.RefreshTokens
                        .Where(x => x.Token == refreshToken &&
                               x.UserId == userId &&
                               x.ExpiryTime >= DateTime.Now)
                        .FirstOrDefaultAsync(cancellationToken);

            await RemoveOldRefreshToken(userId, refreshToken, cancellationToken);


            if (user is null || token is null)
            {
                return null;
            }

            return user;
        }
        private async Task<RefreshToken> RemoveOldRefreshToken(string userId, string refreshToken, CancellationToken cancellationToken = default)
        {
            var OldToken = await context.RefreshTokens
                   .Where(x => x.Token == refreshToken && x.UserId == userId)
                   .FirstOrDefaultAsync(cancellationToken);

            if (OldToken is not null)
            {
                context.RefreshTokens.Remove(OldToken!);
                await context.SaveChangesAsync(cancellationToken);
            }
            return OldToken;
        }
        private async Task<string> GenerateAndSaveRefreshTokensAsync(User user, CancellationToken cancellationToken = default)
        {
            // Generate refresh token
            string refreshToken = GenerateRefreshToken();
            // Save refresh token
            RefreshToken token = new()
            {
                Token = refreshToken,
                UserId = user.Id,
                User = user,
                ExpiryTime = DateTime.Now.AddDays(AuthConstants.RefreshTokenLife),
                CreatedBy = user.FullName,
                CreatedById = user.Id,
                CreatedAt = DateTime.UtcNow,
                ModifiedBy = user.FullName,
                ModifiedById = user.Id,
                ModifiedAt = DateTime.UtcNow,
                IsDeleted = false,
                IpAddress = ""
            };

            await context.RefreshTokens.AddAsync(token, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return token.Token;
        }
        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private static string GenerateTemporaryPassword(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] randomBytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            char[] password = new char[length];
            for (int i = 0; i < length; i++)
                password[i] = chars[randomBytes[i] % chars.Length];
            return new string(password);
        }

        private static bool IsPasswordResetExcludedEmail(string? email) =>
            !string.IsNullOrWhiteSpace(email) &&
            email.Trim().Contains(AccountEmails.InternalDomain, StringComparison.OrdinalIgnoreCase);

        private async Task<string> CreateToken(User user, IEnumerable<Claim> claimDB, CancellationToken cancellationToken = default)
        {
            IList<string> userRoles = await userManager.GetRolesAsync(user);

            IEnumerable<Claim> roles = userRoles.Select(o => new Claim(ClaimTypes.Role, o));

            var role = userRoles.FirstOrDefault() ?? RoleNames.User; // Default to User role if no roles assigned

            IEnumerable<Claim> claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.FullName),
                new Claim(ClaimTypes.Role, role),
                new Claim(AuthConstants.OrgId, ""),
                new Claim(AuthConstants.LeaderId, user.LeaderId ?? ""),
                new Claim(AuthConstants.EmployeeId, user.EmployeeId ?? ""),

            }.Union(claimDB);

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key")!));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken
            (
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(AuthConstants.AccessTokenLife),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private static Expression<Func<User, bool>> PredicateBuilderFunction(FilterUserDto filter)
        {
            var predicate = PredicateBuilder.New<User>(true);
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                predicate = predicate.And(x => x.FullName.Contains(filter.Name));
            }
            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                predicate = predicate.And(x => x.Email.Equals(filter.Email));
            }


            return predicate;
        }

        private IFinalResult UploadResponse(string res)
        {
            if (res == "Size")
            {
                const string message = "File Size Larger than 5 Mega Bytes";
                return new ResponseResult().PostResult(result: null, status: HttpStatusCode.BadRequest, message: message);
            }

            if (res == "Type")
            {
                const string message = "File type not allowed.";
                return new ResponseResult().PostResult(result: null, status: HttpStatusCode.BadRequest, message: message);
            }

            return null;
        }
    }
}

