namespace EventManager.Server.Interfaces
{
    public interface IAuthorizationService
    {
        Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password);
        Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
    }
}
