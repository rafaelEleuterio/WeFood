using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Auth
{
    public class RefreshTokenService
    {
        private readonly WeFoodDbContext _context;

        public RefreshTokenService(WeFoodDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateAsync(Guid userId)
        {
            var token = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<RefreshToken?> GetAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token && !x.IsRevoked);
        }

        public async Task RevokeAsync(string token)
        {
            var existing = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);

            if (existing is null) return;

            existing.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }

}
