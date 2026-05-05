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
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Infrastructure.Context;
using SonoTracker.Application.Services.Identity.Account;
using SonoTracker.Common.Constants.Auth;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Identity.User;
using SonoTracker.Common.Infrastructure.UnitOfWork;
using SonoTracker.Domain;

namespace SonoBooking.Application.Services.Identity.Account
{

    public class AccountService(
                 UserManager<User> userManager,
                 RoleManager<Role> roleManager,
                 SonoBookingDbContext context,
                 UserDataDto auditUser,
                 IConfiguration configuration,
                 IUnitOfWork<User> UnitOfWork,
                 IMapper Mapper) : IAccountService
    {
        public async Task<IFinalResult> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            User checkUser = await userManager.FindByEmailAsync(request.Email);
            
            if (checkUser == null)
            {
                User user = new()
                {
                    Email = request.Email,
                    UserName = request.Email,
                    FullName = request.Username,
                    Gender = Gender.Male,
                    BirthDate = DateOnly.FromDateTime(DateTime.UtcNow),
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

                    if (!result.Succeeded)
                        return responseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.AddError + "User : " +
                                                         string.Join(", ", result.Errors.Select(e => e.Description)));

                    return responseResult.PostResult(result: true, status: HttpStatusCode.Created, exception: null,
                                                     message: MessagesConstants.AddSuccess);
                }
                else
                {
                    Role role = await roleManager.FindByNameAsync(Roles.User);

                    IdentityResult res = await userManager.AddToRoleAsync(user, role.Name!);

                    if (!result.Succeeded)
                        return responseResult.PostResult(user, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.AddError + "User : " +
                                                         string.Join(", ", result.Errors.Select(e => e.Description)));

                    return  responseResult.PostResult(result: true, status: HttpStatusCode.Created, exception:null,
                                                      message: MessagesConstants.AddSuccess); 
                }
            }

            return responseResult.PostResult(result: null,status: HttpStatusCode.Conflict, exception: null,
                                             message: MessagesConstants.Existed);
        }
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

            response = await CreateTokenResponse(user, cancellationToken);

            return responseResult.PostResult(result: response, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
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
                    user.IsLogedIn = false;
                    await userManager.UpdateAsync(user);
                    return responseResult.PostResult(result: true, status: HttpStatusCode.OK, exception: null,
                                                     message: MessagesConstants.Success);
                }
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

            user.Email = updateUser.Email;
            user.UserName = updateUser.Email;
            user.FullName = updateUser.UserName;

            user.ModifiedById = auditUser.Name != "" ? auditUser.Name : user.FullName;
            user.ModifiedAt = DateTime.UtcNow;

            if (updateUser.NewPassword != "" && updateUser.NewPassword is not null)
            {
                if (auditUser.Role == Roles.SuperAdmin)
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

            Role role = await roleManager.FindByIdAsync(updateUser.RoleId.ToString());

            if (role != null)
            {
                IList<string> userRole = await userManager.GetRolesAsync(user);

                if (userRole.Count > 0)
                {
                    await userManager.RemoveFromRolesAsync(user, userRole);
                }

                await userManager.AddToRoleAsync(user, role.Name);

                IdentityResult res = await userManager.UpdateAsync(user);

                if (!res.Succeeded)

                    return responseResult.PostResult(user, status: HttpStatusCode.BadRequest,
                                message: "Failed to Update User : " +
                                string.Join(", ", res.Errors.Select(e => e.Description)));
            }

            return responseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                             message: MessagesConstants.UpdateSuccess);
        }
        public async Task<IFinalResult> GetUserByIdAsync(string Id, CancellationToken cancellationToken = default)
        {
            ResponseResult responseResult = new();

            User user = await userManager.FindByIdAsync(Id);

            UserDto userDto = new()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.FullName,
                Role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "",
                FloatingUnitId = "",
                OrganizationId = "",
            };

            userDto.RoleId = roleManager.Roles.Where(r => r.Name == userDto.Role)
                                              .Select(r => r.Id).FirstOrDefault() ?? "";

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
                    OrganizationId = "",
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
        private async Task<string> CreateToken(User user, IEnumerable<Claim> claimDB, CancellationToken cancellationToken = default)
        {
            IList<string> userRoles = await userManager.GetRolesAsync(user);
            
            IEnumerable<Claim> roles = userRoles.Select(o => new Claim(ClaimTypes.Role, o));
          
            var role = userRoles.FirstOrDefault() ?? Roles.User; // Default to User role if no roles assigned
            
            IEnumerable<Claim> claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.FullName),
                new Claim(ClaimTypes.Role, role),
                new Claim(AuthConstants.OrgId, ""),
                new Claim(AuthConstants.FloatingUnitId, ""),

            }.Union(claimDB);

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key")!));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken
            (
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(AuthConstants.AccessTokenLifeInHours),
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
    }
}
