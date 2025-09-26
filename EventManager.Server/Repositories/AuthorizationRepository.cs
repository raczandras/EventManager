using EventManager.Server.Data.Context;
using EventManager.Server.Data.Entities;
using EventManager.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Server.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthorizationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveRefreshTokenAsync(RefreshToken token)
        {
            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens.Include(rt => rt.User).AsNoTracking().FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
        }

        public async Task InvalidateRefreshTokenAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            _dbContext.RefreshTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
