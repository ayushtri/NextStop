using Microsoft.EntityFrameworkCore;
using NextStopApp.Data;
using NextStopApp.DTOs;

namespace NextStopApp.Repositories
{
    public class TokenService : ITokenService
    {
        private readonly NextStopDbContext _context;

        public TokenService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task SaveRefreshToken(string username, string token)
        {
            var refreshToken = new RefreshToken
            {
                Username = username,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();
        }

        public async Task<string> RetrieveEmailByRefreshToken(string refreshToken)
        {
            var tokenRecord = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow);

            return tokenRecord?.Username;
        }

        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            var tokenRecord = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenRecord != null)
            {
                _context.RefreshTokens.Remove(tokenRecord);

                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
