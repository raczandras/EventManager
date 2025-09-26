using EventManager.Server.Data.Context;
using EventManager.Server.Data.Entities;
using EventManager.Server.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Repositories
{
    public class AuthorizationRepositoryTests
    {
        private ApplicationDbContext _context = null!;
        private AuthorizationRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new AuthorizationRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task SaveRefreshTokenAsync_SavesTokenSuccessfully()
        {
            // Arrange
            var token = new RefreshToken
            {
                Token = "test-token",
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = "1"
            };

            // Act
            await _repository.SaveRefreshTokenAsync(token);

            // Assert
            var saved = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == "test-token");
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.UserId, Is.EqualTo("1"));
        }

        [Test]
        public async Task GetRefreshTokenAsync_WhenTokenExists_ReturnsToken()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user" };
            _context.Users.Add(user);

            var token = new RefreshToken
            {
                Token = "existing-token",
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                User = user
            };
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRefreshTokenAsync("existing-token");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Token, Is.EqualTo("existing-token"));
            Assert.That(result.User, Is.Not.Null);
            Assert.That(result.User.UserName, Is.EqualTo("user"));
        }

        [Test]
        public async Task GetRefreshTokenAsync_WhenTokenDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _repository.GetRefreshTokenAsync("missing-token");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task InvalidateRefreshTokenAsync_SetsTokenAsRevoked()
        {
            // Arrange
            var token = new RefreshToken
            {
                Token = "to-revoke",
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = "1"
            };
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            // Act
            await _repository.InvalidateRefreshTokenAsync(token);

            // Assert
            var saved = await _context.RefreshTokens.FirstAsync(t => t.Token == "to-revoke");
            Assert.That(saved.IsRevoked, Is.True);
        }
    }
}
