using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EventManager.Server.Data.Entities;
using EventManager.Server.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EventManager.Server.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly ILogger<AuthorizationService> _logger;

        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _accessTokenLifetimeMinutes;
        private readonly int _refreshTokenLifetimeDays;

        public AuthorizationService(UserManager<IdentityUser> userManager, IConfiguration config, IAuthorizationRepository authorizationRepository, ILogger<AuthorizationService> logger)
        {
            _userManager = userManager;
            _authorizationRepository = authorizationRepository;
            _logger = logger;

            _jwtKey = config["Jwt:Key"] ?? throw new InvalidOperationException("Missing configuration: Jwt:Key");
            _jwtIssuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing configuration: Jwt:Issuer");
            _jwtAudience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Missing configuration: Jwt:Audience");

            if (!int.TryParse(config["Jwt:AccessTokenLifetimeMinutes"], out _accessTokenLifetimeMinutes))
            {
                throw new InvalidOperationException("Missing or invalid configuration: Jwt:AccessTokenLifetimeMinutes");
            }

            if (!int.TryParse(config["Jwt:RefreshTokenLifetimeDays"], out _refreshTokenLifetimeDays))
            {
                throw new InvalidOperationException("Missing or invalid configuration: Jwt:RefreshTokenLifetimeDays");
            }
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password)
        {
            _logger.LogInformation("Login attempt for user {Email}", email);
            var sw = Stopwatch.StartNew();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning("Login failed for user {email}: invalid credentials", email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var accessToken = await GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            await _authorizationRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenLifetimeDays),
                UserId = user.Id
            });

            sw.Stop();
            _logger.LogInformation("Login successful for user {Username}. Elapsed: {Elapsed}ms", email, sw.ElapsedMilliseconds);

            return (accessToken, refreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken)
        {
            _logger.LogInformation("Refresh token attempt");
            var sw = Stopwatch.StartNew();

            var token = await _authorizationRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null || token.Expires <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token failed: invalid or expired token");
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            await _authorizationRepository.InvalidateRefreshTokenAsync(token);

            var accessToken = await GenerateAccessToken(token.User!);
            var newRefreshToken = GenerateRefreshToken();

            await _authorizationRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenLifetimeDays),
                UserId = token.UserId
            });

            sw.Stop();
            _logger.LogInformation("Refresh token succeeded for user {UserId}. Elapsed: {Elapsed}ms", token.UserId, sw.ElapsedMilliseconds);

            return (accessToken, newRefreshToken);
        }

        private async Task<string> GenerateAccessToken(IdentityUser user)
        {
            var sw = Stopwatch.StartNew();

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenLifetimeMinutes),
                signingCredentials: creds);

            sw.Stop();
            _logger.LogInformation("Generated access token for user {UserId}. Elapsed: {Elapsed}ms", user.Id, sw.ElapsedMilliseconds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _authorizationRepository.GetRefreshTokenAsync(refreshToken);

            if (token == null || token.Expires <= DateTime.UtcNow || token.IsRevoked == true)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            await _authorizationRepository.InvalidateRefreshTokenAsync(token);

            _logger.LogInformation("User {UserId} logged out, refresh token revoked", token.UserId);
        }
    }
}
