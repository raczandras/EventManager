using EventManager.Server.Data.Entities;
using EventManager.Server.Interfaces;
using EventManager.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Services
{
    public class AuthorizationServiceTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock = null!;
        private Mock<IAuthorizationRepository> _authorizationRepositoryMock = null!;
        private AuthorizationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = MockUserManager();
            _authorizationRepositoryMock = new Mock<IAuthorizationRepository>();

            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "this_is_a_super_secret_key_32_chars_minimum_you_find_out_the_hard_way" },
                { "Jwt:Issuer", "EventManagerServer" },
                { "Jwt:Audience", "EventManagerClient" },
                { "Jwt:AccessTokenLifetimeMinutes", "15" },
                { "Jwt:RefreshTokenLifetimeDays", "7" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            var logger = Mock.Of<ILogger<AuthorizationService>>();

            _service = new AuthorizationService(_userManagerMock.Object, config, _authorizationRepositoryMock.Object, logger);
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsTokens()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user", Email = "user@test.com" };

            _userManagerMock.Setup(m => m.FindByNameAsync("user")).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true);
            _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var (accessToken, refreshToken) = await _service.LoginAsync("user", "password");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(accessToken, Is.Not.Empty);
                Assert.That(refreshToken, Is.Not.Empty);
            });

            _authorizationRepositoryMock.Verify(r => r.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Test]
        public void Login_WithInvalidCredentials_ThrowsUnauthorized()
        {
            // Arrange
            _userManagerMock.Setup(m => m.FindByNameAsync("user")).ReturnsAsync((IdentityUser?)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _service.LoginAsync("user", "password"));
        }

        [Test]
        public async Task Refresh_WithValidToken_ReturnsNewTokens()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user", Email = "user@test.com" };
            var validToken = new RefreshToken
            {
                Token = "old-refresh",
                Expires = DateTime.UtcNow.AddDays(1),
                UserId = user.Id,
                User = user
            };

            _authorizationRepositoryMock.Setup(r => r.GetRefreshTokenAsync("old-refresh")).ReturnsAsync(validToken);

            _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var (accessToken, refreshToken) = await _service.RefreshAsync("old-refresh");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(accessToken, Is.Not.Empty);
                Assert.That(refreshToken, Is.Not.Empty);
                Assert.That(refreshToken, Is.Not.EqualTo("old-refresh")); //should be new
            });

            _authorizationRepositoryMock.Verify(r => r.InvalidateRefreshTokenAsync(validToken), Times.Once);
            _authorizationRepositoryMock.Verify(r => r.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Test]
        public void Refresh_WithInvalidToken_ThrowsUnauthorized()
        {
            // Arrange
            _authorizationRepositoryMock.Setup(r => r.GetRefreshTokenAsync("bad-token")).ReturnsAsync((RefreshToken?)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _service.RefreshAsync("bad-token"));
        }

        [Test]
        public async Task Logout_WithValidToken_RevokesRefreshToken()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user", Email = "user@test.com" };
            var validToken = new RefreshToken
            {
                Token = "valid-refresh",
                Expires = DateTime.UtcNow.AddDays(1),
                UserId = user.Id,
                User = user,
                IsRevoked = false
            };

            _authorizationRepositoryMock.Setup(r => r.GetRefreshTokenAsync("valid-refresh"))
                .ReturnsAsync(validToken);

            // Act
            await _service.LogoutAsync("valid-refresh");

            // Assert
            _authorizationRepositoryMock.Verify(r => r.InvalidateRefreshTokenAsync(validToken), Times.Once);
        }

        [Test]
        public void Logout_WithInvalidToken_ThrowsUnauthorized()
        {
            // Arrange
            _authorizationRepositoryMock.Setup(r => r.GetRefreshTokenAsync("invalid-refresh"))
                .ReturnsAsync((RefreshToken?)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.LogoutAsync("invalid-refresh"));
        }


        //Helper to mock UserManager
        private static Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }
    }
}