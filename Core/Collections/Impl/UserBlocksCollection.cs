using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Collections.Impl;

public sealed class UserBlocksCollection(CoreDb db) : IUserBlocks
{
    public Task<bool> AreUsersBlocked(Guid userId, Guid otherUserId, CancellationToken ct) =>
        db.UserBlocks.AnyAsync(b =>
            (b.BlockerUserId == userId && b.BlockedUserId == otherUserId) ||
            (b.BlockerUserId == otherUserId && b.BlockedUserId == userId), ct);

    public async Task<(bool BlockedByMe, bool BlockedMe)> GetStatus(Guid userId, Guid otherUserId, CancellationToken ct)
    {
        var directions = await db.UserBlocks.AsNoTracking()
            .Where(b => (b.BlockerUserId == userId && b.BlockedUserId == otherUserId) ||
                        (b.BlockerUserId == otherUserId && b.BlockedUserId == userId))
            .Select(b => b.BlockerUserId).ToListAsync(ct);
        return (directions.Contains(userId), directions.Contains(otherUserId));
    }

    public async Task Block(Guid userId, Guid otherUserId, CancellationToken ct)
    {
        if (userId == otherUserId || otherUserId == Guid.Empty)
            throw new ArgumentException("No puedes bloquear a este usuario.");
        if (!await db.Users.AnyAsync(u => u.Id == otherUserId, ct))
            throw new KeyNotFoundException("Usuario no encontrado.");
        await db.Database.ExecuteSqlInterpolatedAsync(
            $"""INSERT INTO "UserBlocks" ("BlockerUserId", "BlockedUserId", "CreatedAt") VALUES ({userId}, {otherUserId}, NOW()) ON CONFLICT ("BlockerUserId", "BlockedUserId") DO NOTHING""", ct);
    }

    public async Task Unblock(Guid userId, Guid otherUserId, CancellationToken ct) =>
        await db.UserBlocks.Where(b => b.BlockerUserId == userId && b.BlockedUserId == otherUserId)
            .ExecuteDeleteAsync(ct);
}
