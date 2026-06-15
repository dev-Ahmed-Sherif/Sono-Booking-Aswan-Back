using Microsoft.AspNetCore.Identity;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Domain.Entities.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Identity.Accounts
{
    public interface IAccountService
    {
        Task<IFinalResult> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default);
        Task<IFinalResult> CheckNationalIdExistsAsync(string nationalId, CancellationToken cancellationToken = default);
        Task<IFinalResult> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<IFinalResult> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default);
        Task<IFinalResult> LogoutAsync(string id, CancellationToken cancellationToken = default);
        Task<LoginResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetUsersAsync(CancellationToken cancellationToken = default);
        Task<PagingResult> GetAllPagedAsync(BaseParam<FilterUserDto> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> UpdateUser(UpdateUserDto changePersonalData, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteUser(string userId, CancellationToken cancellationToken = default);
    }
}

