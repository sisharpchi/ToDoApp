using Microsoft.EntityFrameworkCore;
using ToDoList.Dal;
using ToDoList.Dal.Entity;

namespace ToDoList.Repository.ToDoItemRepository;

public class RefreshTokenRepository(MainContext MainContext) : IRefreshTokenRepository
{
    public async Task InsertRefreshTokenAsync(RefreshToken refreshToken)
    {
        await MainContext.RefreshTokens.AddAsync(refreshToken);
        await MainContext.SaveChangesAsync();
    }

    public async Task<RefreshToken> SelectRefreshTokenAsync(string refreshToken, long userId)
    {
        return await MainContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken && r.UserId == userId);
    }
}
