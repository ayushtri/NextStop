namespace NextStopApp.Repositories
{
    public interface ITokenService
    {
        Task SaveRefreshToken(string username, string token);
        Task<string> RetrieveEmailByRefreshToken(string refreshToken);
        Task<bool> RevokeRefreshToken(string refreshToken);
    }
}
