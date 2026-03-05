using Microsoft.AspNetCore.SignalR;
using Core.Models;
using Core.Data;
using System.Collections.Concurrent;
namespace Core.Hubs
{
    public sealed class ChatHub(CoreDb context) : Hub
    {
        public static ConcurrentDictionary<string, Guid> Users = new();
        private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>> _blockedUsers = new();

        public async Task Connect(string userId)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid guidUserId))
            {
                // Notificar intención de conexión pero no registrar si no hay id
                await Clients.Caller.SendAsync("UserConnected", (Guid?)null);
                return;
            }

            await Clients.Caller.SendAsync("UserConnected", guidUserId);
            Users.TryAdd(Context.ConnectionId, guidUserId);

            User? user = await context.Users.FindAsync(guidUserId);
            if (user is not null)
            {
                user.Status = "online";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                //await Clients.All.SendAsync("Users", user);

                // Si el usuario tiene bloqueos guardados en la base de datos, inicializamos el conjunto local
                if (user.BlockedUsers is not null && user.BlockedUsers.Length > 0)
                {
                    var blockedSet = _blockedUsers.GetOrAdd(guidUserId, _ => new ConcurrentDictionary<Guid, byte>());
                    foreach (var blockedId in user.BlockedUsers)
                    {
                        if (Guid.TryParse(blockedId, out Guid blockedGuid))
                        {
                            blockedSet.TryAdd(blockedGuid, 0);
                        }
                    }

                    // Notificar al cliente que los bloqueos han sido inicializados
                    await Clients.Caller.SendAsync("BlockedUsersInitialized", user.BlockedUsers);
                }
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

            //await Clients.Caller.SendAsync("UserDisconnected", userId); // Notifica a todos los clientes
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "offline";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToCaller(string userId, string toUserId, string status)
        {
            if (!Guid.TryParse(toUserId, out Guid toGuid))
                return;
            var connectionId = Users.FirstOrDefault(p => p.Value == toGuid).Key;
            if (!string.IsNullOrEmpty(connectionId))
            {
                await Clients.Client(connectionId).SendAsync("UserStatus", userId, status);
            }
        }

        public async Task MarkMessageAsReceived(string messageId, string receiverId)
        {
            if (!Guid.TryParse(receiverId, out Guid receiverGuid))
                return;
            // Busca la conexión del usuario receptor
            var receiverConnectionId = Users.FirstOrDefault(p => p.Value == receiverGuid).Key;

            if (string.IsNullOrEmpty(receiverConnectionId))
                return;

            // Envía el evento al usuario específico
            await Clients.Client(receiverConnectionId).SendAsync("MessageReceived", messageId);
        }












        // Método para bloquear a un usuario (validado y thread-safe)
        public async Task BlockUser(string userIdToBlock)
        {
            if (string.IsNullOrWhiteSpace(userIdToBlock) || !Guid.TryParse(userIdToBlock, out Guid guidToBlock))
            {
                await Clients.Caller.SendAsync("UserBlockError", "invalid_user_id");
                return;
            }

            // Obtén el ID del usuario actual desde el mapa de conexiones; si no existe, usa UserIdentifier como fallback
            var currentUserId = Users.TryGetValue(Context.ConnectionId, out var uid) ? uid : (Guid.TryParse(Context.UserIdentifier, out Guid parsed) ? parsed : Guid.Empty);

            if (currentUserId == Guid.Empty)
            {
                await Clients.Caller.SendAsync("UserBlockError", "current_user_not_found");
                return;
            }

            if (currentUserId == guidToBlock)
            {
                await Clients.Caller.SendAsync("UserBlockError", "cannot_block_self");
                return;
            }

            var blockedSet = _blockedUsers.GetOrAdd(currentUserId, _ => new ConcurrentDictionary<Guid, byte>());

            // Evita duplicados
            if (!blockedSet.TryAdd(guidToBlock, 0))
            {
                await Clients.Caller.SendAsync("UserBlocked", userIdToBlock, "already_blocked");
                return;
            }

            // Intentar localizar la conexión del usuario afectado y actualizar chatmap por si escribe
            var targetConnectionId = Users.FirstOrDefault(p => p.Value == guidToBlock).Key;
            if (!string.IsNullOrWhiteSpace(targetConnectionId))
            {
                await Clients.Caller.SendAsync("UserBlocked", currentUserId);
            }
        }

        // Método para verificar si un usuario está bloqueado por otro
        public async Task IsUserBlocked(string senderId, string? receiverId = null)
        {
            if (string.IsNullOrWhiteSpace(senderId) || !Guid.TryParse(senderId, out Guid senderGuid))
            {
                await Clients.Caller.SendAsync("MessageBlocked", false);
                return;
            }

            var targetReceiverId = receiverId;
            if (string.IsNullOrWhiteSpace(targetReceiverId) || !Guid.TryParse(targetReceiverId, out Guid receiverGuid))
            {
                receiverGuid = Users.TryGetValue(Context.ConnectionId, out var uid) ? uid : (Guid.TryParse(Context.UserIdentifier, out Guid parsed) ? parsed : Guid.Empty);
                if (receiverGuid == Guid.Empty)
                {
                    await Clients.Caller.SendAsync("MessageBlocked", false);
                    return;
                }
            }

            if (_blockedUsers.TryGetValue(receiverGuid, out var blockedSet) && blockedSet.ContainsKey(senderGuid))
            {
                await Clients.Caller.SendAsync("MessageBlocked", true);
            }
            else
            {
                await Clients.Caller.SendAsync("MessageBlocked", false);
            }
        }

        // Método para desbloquear a un usuario (validado y thread-safe)
        public async Task UnblockUser(string userIdToUnblock)
        {
            if (string.IsNullOrWhiteSpace(userIdToUnblock) || !Guid.TryParse(userIdToUnblock, out Guid guidToUnblock))
            {
                await Clients.Caller.SendAsync("UserUnblockError", "invalid_user_id");
                return;
            }

            var currentUserId = Users.TryGetValue(Context.ConnectionId, out var uid) ? uid : (Guid.TryParse(Context.UserIdentifier, out Guid parsed) ? parsed : Guid.Empty);

            if (currentUserId == Guid.Empty)
            {
                await Clients.Caller.SendAsync("UserUnblockError", "current_user_not_found");
                return;
            }

            if (_blockedUsers.TryGetValue(currentUserId, out var blockedSet))
            {
                blockedSet.TryRemove(guidToUnblock, out _);
                if (blockedSet.IsEmpty)
                {
                    _blockedUsers.TryRemove(currentUserId, out _);
                }
            }

            // Intentar localizar la conexión del usuario afectado y actualizar chatmap por si escribe
            var targetConnectionId = Users.FirstOrDefault(p => p.Value == guidToUnblock).Key;
            if (!string.IsNullOrWhiteSpace(targetConnectionId))
            {
                await Clients.Client(targetConnectionId).SendAsync("UserUnblocked", currentUserId);
            }
        }

        // Sobrescribe OnConnected para manejar conexiones
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }

}