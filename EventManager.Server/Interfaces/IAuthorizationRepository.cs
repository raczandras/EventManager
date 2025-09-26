using EventManager.Server.Data.Entities;

namespace EventManager.Server.Interfaces
{
    public interface IAuthorizationRepository
    {
        Task SaveRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task InvalidateRefreshTokenAsync(RefreshToken token);
    }
}
