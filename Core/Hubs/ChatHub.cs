using Microsoft.AspNetCore.SignalR;
using Core.Models;
using Core.Data;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace Core.Hubs
{
    [Authorize]
    public sealed class ChatHub(CoreDb context) : Hub
    {
        public static ConcurrentDictionary<string, Guid> Users = new();

        public async Task Connect(string userId)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid guidUserId) ||
                !Guid.TryParse(Context.UserIdentifier, out var authenticatedUserId) || authenticatedUserId != guidUserId)
            {
                // Notificar intención de conexión pero no registrar si no hay id
                await Clients.Caller.SendAsync("UserConnected", (Guid?)null);
                return;
            }

            Users[Context.ConnectionId] = guidUserId;
            await Clients.Caller.SendAsync("UserConnected", guidUserId);
            await Clients.Caller.SendAsync("OnlineUsers", Users.Values.Distinct().Where(id => id != guidUserId));
            await Clients.Others.SendAsync("UserStatus", userId, "online");

            User? user = await context.Users.FindAsync(guidUserId);
            if (user is not null)
            {
                user.Status = "online";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                //await Clients.All.SendAsync("Users", user);


            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Users.TryRemove(Context.ConnectionId, out var userId);

            if (userId == Guid.Empty)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            // Otra pestaña/conexión del mismo usuario sigue activa
            if (Users.Values.Contains(userId))
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "offline";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("UserStatus", userId.ToString(), "offline");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToCaller(string userId, string toUserId, string status)
        {
            if (!Guid.TryParse(Context.UserIdentifier, out var currentUserId) ||
                !Guid.TryParse(userId, out var fromId) || currentUserId != fromId ||
                !Guid.TryParse(toUserId, out Guid toGuid))
                return;
            var connectionId = Users.FirstOrDefault(p => p.Value == toGuid).Key;
            if (!string.IsNullOrEmpty(connectionId))
            {
                await Clients.Client(connectionId).SendAsync("UserStatus", userId, status);
            }
        }

        public async Task MarkMessageAsReceived(string messageId, string senderId)
        {
            if (!Guid.TryParse(messageId, out var id) || !Guid.TryParse(senderId, out Guid senderGuid) ||
                !Guid.TryParse(Context.UserIdentifier, out var recipientId) ||
                !await context.Chats.AsNoTracking().AnyAsync(m =>
                    m.Id == id && m.UserId == senderGuid && m.ToUserId == recipientId &&
                    (m.State == "Received" || m.State == "Read")))
                return;
            var senderConnectionId = Users.FirstOrDefault(p => p.Value == senderGuid).Key;

            if (string.IsNullOrEmpty(senderConnectionId))
                return;

            await Clients.Client(senderConnectionId).SendAsync("MessageReceived", messageId);
        }

        public async Task MarkMessageAsRead(string messageId, string senderId)
        {
            if (!Guid.TryParse(messageId, out var id) || !Guid.TryParse(senderId, out Guid senderGuid) ||
                !Guid.TryParse(Context.UserIdentifier, out var recipientId) ||
                !await context.Chats.AsNoTracking().AnyAsync(m =>
                    m.Id == id && m.UserId == senderGuid && m.ToUserId == recipientId && m.State == "Read"))
                return;

            var connectionId = Users.FirstOrDefault(x => x.Value == senderGuid).Key;

            if (!string.IsNullOrEmpty(connectionId))
            {
                await Clients.Client(connectionId)
                    .SendAsync(
                        "MessageRead",
                        new[] { messageId }
                    );
            }
        }












        // Sobrescribe OnConnected para manejar conexiones
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }

}