using EventManager.Server.ApiModels.Authorization;
using EventManager.Server.Controllers;
using EventManager.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers
{
    public class AuthorizationControllerTests
    {
        private Mock<IAuthorizationService> _authorizationServiceMock = null!;
        private AuthorizationController _authorizationController = null!;

        [SetUp]
        public void Setup()
        {
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _authorizationController = new AuthorizationController(_authorizationServiceMock.Object);
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
        {
            // Arrange
            var dto = new LoginDto { Email = "user@user.com", Password = "password" };
            _authorizationServiceMock
                .Setup(s => s.LoginAsync(dto.Email, dto.Password))
                .ReturnsAsync(("access-token", "refresh-token"));

            // Act
            var result = await _authorizationController.Login(dto);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            // Access anonymous object via reflection
            var tokenProp = okResult.Value?.GetType().GetProperty("Token")?.GetValue(okResult.Value, null);
            var refreshProp = okResult.Value?.GetType().GetProperty("RefreshToken")?.GetValue(okResult.Value, null);

            Assert.Multiple(() =>
            {
                Assert.That(tokenProp, Is.EqualTo("access-token"));
                Assert.That(refreshProp, Is.EqualTo("refresh-token"));
            });
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDto { Email = "user@user.com", Password = "wrong" };
            _authorizationServiceMock
                .Setup(s => s.LoginAsync(dto.Email, dto.Password))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _authorizationController.Login(dto);

            // Assert
            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult, Is.Not.Null);

            var messageProp = unauthorizedResult.Value?.GetType().GetProperty("Message")?.GetValue(unauthorizedResult.Value, null);
            Assert.That(messageProp, Is.EqualTo("Invalid credentials"));
        }

        [Test]
        public async Task Refresh_WithValidToken_ReturnsOkWithTokens()
        {
            // Arrange
            var dto = new RefreshTokenDto { RefreshToken = "valid-refresh" };
            _authorizationServiceMock
                .Setup(s => s.RefreshAsync(dto.RefreshToken))
                .ReturnsAsync(("new-access", "new-refresh"));

            // Act
            var result = await _authorizationController.Refresh(dto);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var tokenProp = okResult.Value?.GetType().GetProperty("Token")?.GetValue(okResult.Value, null);
            var refreshProp = okResult.Value?.GetType().GetProperty("RefreshToken")?.GetValue(okResult.Value, null);

            Assert.Multiple(() =>
            {
                Assert.That(tokenProp, Is.EqualTo("new-access"));
                Assert.That(refreshProp, Is.EqualTo("new-refresh"));
            });
        }

        [Test]
        public async Task Refresh_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new RefreshTokenDto { RefreshToken = "invalid-refresh" };
            _authorizationServiceMock
                .Setup(s => s.RefreshAsync(dto.RefreshToken))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid or expired refresh token"));

            // Act
            var result = await _authorizationController.Refresh(dto);

            // Assert
            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult, Is.Not.Null);

            var messageProp = unauthorizedResult.Value?.GetType().GetProperty("Message")?.GetValue(unauthorizedResult.Value, null);
            Assert.That(messageProp, Is.EqualTo("Invalid or expired refresh token"));
        }

        [Test]
        public async Task Logout_WithValidToken_ReturnsNoContent()
        {
            // Arrange
            var dto = new RefreshTokenDto { RefreshToken = "valid-refresh" };
            _authorizationServiceMock.Setup(s => s.LogoutAsync(dto.RefreshToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authorizationController.Logout(dto);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Logout_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new RefreshTokenDto { RefreshToken = "invalid-refresh" };
            _authorizationServiceMock.Setup(s => s.LogoutAsync(dto.RefreshToken))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid or expired refresh token"));

            // Act
            var result = await _authorizationController.Logout(dto);

            // Assert
            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult, Is.Not.Null);

            var messageProp = unauthorizedResult!.Value?.GetType().GetProperty("Message")?.GetValue(unauthorizedResult.Value, null);
            Assert.That(messageProp, Is.EqualTo("Invalid or expired refresh token"));
        }
    }
}
