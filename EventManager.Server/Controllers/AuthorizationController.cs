using EventManager.Server.ApiModels.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly Interfaces.IAuthorizationService _authorizationService;

        public AuthorizationController(Interfaces.IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Authenticate a user and return an access token and refresh token.
        /// </summary>
        /// <param name="dto">The login credentials (username and password).</param>
        /// <returns>Access token and refresh token if credentials are valid, 401 Unauthorized if not.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var (accessToken, refreshToken) = await _authorizationService.LoginAsync(dto.Email, dto.Password);

                return Ok(new { Token = accessToken, RefreshToken = refreshToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        /// <summary>
        /// Refresh an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="dto">The refresh token request.</param>
        /// <returns>A new access token and refresh token.</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
        {
            try
            {
                var (accessToken, refreshToken) = await _authorizationService.RefreshAsync(dto.RefreshToken);

                return Ok(new { Token = accessToken, RefreshToken = refreshToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        /// <summary>
        /// Logout a user and invalidate the refresh token.
        /// </summary>
        /// <param name="dto">The refresh token request.</param>
        /// <returns>No content response.</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(RefreshTokenDto dto)
        {
            try
            {
                await _authorizationService.LogoutAsync(dto.RefreshToken);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }
    }
}
