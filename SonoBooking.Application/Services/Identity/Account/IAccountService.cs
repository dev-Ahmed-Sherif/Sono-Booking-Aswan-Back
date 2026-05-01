using Microsoft.AspNetCore.Identity;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Identity.User;
using SonoTracker.Domain.Entities.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Identity.Account
{
    public interface IAccountService
    {
        Task<IFinalResult> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default);
        Task<IFinalResult> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<IFinalResult> LogoutAsync(string id, CancellationToken cancellationToken = default);
        Task<LoginResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetUsersAsync(CancellationToken cancellationToken = default);
        Task<PagingResult> GetAllPagedAsync(BaseParam<FilterUserDto> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> UpdateUser(UpdateUserDto changePersonalData, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteUser(string userId, CancellationToken cancellationToken = default);
    }
}
