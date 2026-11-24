namespace Core.Collections;

public interface IUserBlocks
{
    Task<bool> AreUsersBlocked(Guid userId, Guid otherUserId, CancellationToken ct);
    Task<(bool BlockedByMe, bool BlockedMe)> GetStatus(Guid userId, Guid otherUserId, CancellationToken ct);
    Task Block(Guid userId, Guid otherUserId, CancellationToken ct);
    Task Unblock(Guid userId, Guid otherUserId, CancellationToken ct);
}
