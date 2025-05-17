using Microsoft.EntityFrameworkCore;
using ToDoList.Dal;
using ToDoList.Dal.Entity;
using ToDoList.Errors;

namespace ToDoList.Repository.ToDoItemRepository;

public class RefreshTokenRepository(MainContext MainContext) : IRefreshTokenRepository
{
    public async Task InsertRefreshTokenAsync(RefreshToken refreshToken)
    {
        await MainContext.RefreshTokens.AddAsync(refreshToken);
        await MainContext.SaveChangesAsync();
    }

    public async Task RemoveRefreshTokenAsync(string refreshToken)
    {
        var refreshTokenToRemove = await MainContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (refreshTokenToRemove is null) 
            throw new EntityNotFoundException($"Refresh token {refreshToken} not found");
        MainContext.RefreshTokens.Remove(refreshTokenToRemove);
        await MainContext.SaveChangesAsync();
    }

    public async Task<RefreshToken> SelectRefreshTokenAsync(string refreshToken, long userId)
    {
        return await MainContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken && r.UserId == userId);
    }
}
