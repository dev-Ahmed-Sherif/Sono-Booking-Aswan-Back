using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Api.Controllers.V1.Base;
using System.Net;
using SonoBooking.Application.Services.Identity.Account;
using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;

namespace SonoBooking.Api.Controllers.V1.Identity
{
    /// <summary>
    /// Controller for managing user accounts, including registration, login, and role updates.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AccountsController"/> class.
    /// </remarks>
    /// <param name="accountService">Service for handling account-related operations.</param>
    /// <param name="userData">Data of the currently logged-in user.</param>

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController(IAccountService accountService, UserDataDto userData) : BaseController
    {

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="user">The registration details of the user.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a success message if the registration is successful, 
        /// or a conflict response if the registration fails.
        /// </returns>
        
        [HttpPost("register")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IFinalResult>> RegisterAsync([FromBody] RegisterDto user, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await accountService.RegisterAsync(user, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            
            if (res.Status == HttpStatusCode.Conflict) return Conflict(res);

            return Ok(res);
        }

        /// <summary>
        /// Authenticates a user and generates an access token and refresh token.
        /// </summary>
        /// <param name="user">The login request containing the user's email and password.</param>
        /// <returns>
        /// An <see cref="IFinalResult"/> containing a <see cref="IFinalResult"/> if the login is successful, 
        /// or an unauthorized response if the login fails.
        /// </returns>
        
        [HttpPost("login")]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IFinalResult>> LoginAsync([FromBody] LoginRequestDto user, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await accountService.LoginAsync(user, cancellationToken);

            if (res.Status == HttpStatusCode.Unauthorized)  return Unauthorized(res);
            
            return Ok(res);
        }

        /// <summary>
        /// Logs out the user with the specified user ID.
        /// </summary>
        /// <returns>An IActionResult indicating the result of the logout operation.</returns>
        
        [HttpPost("logout"), Authorize]
        //[HttpGet("logout"), Authorize]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IFinalResult>> Logout(Guid id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await accountService.LogoutAsync(userData.Id, cancellationToken);

            if (res.Status == HttpStatusCode.Unauthorized) return Unauthorized(res);

            return Ok(res);
        }

        /// <summary>
        /// Refreshes the access token and refresh token for the user.
        /// </summary>
        /// <param name="model">The refresh token request containing user ID, access token, and refresh token.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a <see cref="LoginResponseDto"/> if successful, 
        /// or an unauthorized response if the refresh operation fails.
        /// </returns>
        
        [HttpPost("refresh"), Authorize]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IFinalResult>> RefreshTokenAsync([FromBody] RefreshTokenRequestDto model, CancellationToken cancellationToken = default)
        {
            var responseResult = new ResponseResult();

            var loginResult = await accountService.RefreshTokensAsync(model, cancellationToken);

            if (loginResult?.IsLogedIn != true)
            {
                return Unauthorized(responseResult.PostResult(loginResult,
                                    HttpStatusCode.Unauthorized,
                                    message: HttpStatusCode.Unauthorized.ToString()));
            }

            return Ok(responseResult.PostResult(loginResult,
                      HttpStatusCode.OK,
                      message: HttpStatusCode.OK.ToString()));
        }

        /// <summary>
        /// Retrieves the data of the currently logged-in user from the access token.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> containing the <see cref="UserDataDto"/> of the logged-in user.</returns>
        
        [HttpGet("tokenData"), Authorize]
        public ActionResult<IFinalResult> AccessTokenData()
        {
            var responseResult = new ResponseResult();

            return Ok(responseResult.PostResult(userData, HttpStatusCode.OK,
                      message: HttpStatusCode.OK.ToString()));
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the <see cref="User"/> object if found, 
        /// or a not found response if the user does not exist.
        /// </returns>
        [HttpGet("get/{id}"), Authorize]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await accountService.GetUserByIdAsync(id.ToString(), cancellationToken);
            return Ok(res);
        }
        
        /// <summary>
        /// Gets all users in the application.
        /// </summary>
        /// <returns>A list of users.</returns>
        
        [HttpGet("getAllUsers"), Authorize]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            IFinalResult users = await accountService.GetUsersAsync(cancellationToken);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a paginated list of users in the application.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a paginated list of users if found, 
        /// or a not found response if no users exist.
        /// </returns>
        [HttpPost("getPaged"), Authorize]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IFinalResult>> GetAllPagedAsync(BaseParam<FilterUserDto> filter, CancellationToken cancellationToken = default)
        {
            var users = await accountService.GetAllPagedAsync(filter, cancellationToken);
            return Ok(users);
        }

        /// <summary>
        /// Updates the personal data of the currently logged-in user.
        /// </summary>
        /// <param name="changeUserPersonalDataDto">The DTO containing the user's updated personal data.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation. 
        /// Returns a success message if the update is successful, or a bad request response if the operation fails.
        /// </returns>
        
        [HttpPut("updateUserPersonalData"), Authorize]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> UpdateUserPersonalData([FromBody] UpdateUserDto changeUserPersonalDataDto, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await accountService.UpdateUser(changeUserPersonalDataDto, cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);

            return Accepted(res);
        }

        /// <summary>
        /// Removes a user by their unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating whether the user was successfully removed.
        /// </returns>

        [HttpDelete("delete/{id}"), Authorize]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status202Accepted)]
        [ProducesResponseType<IFinalResult>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IFinalResult>> DelereUser(Guid id, CancellationToken cancellationToken = default)
        {
            IFinalResult res = await accountService.DeleteUser(id.ToString(), cancellationToken);

            if (res.Status == HttpStatusCode.BadRequest) return BadRequest(res);
            
            return Ok(res);
        }
    }
}

