using Microsoft.AspNetCore.SignalR;
using Users.Models;
using Users.Data;
using System.Collections.Concurrent;
namespace Users.Hubs
{
    public sealed class ChatHub(UserDb context) : Hub
    {
        public static ConcurrentDictionary<string, string> Users = new();
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _blockedUsers = new();

        public async Task Connect(string userId)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(userId))
            {
                // Notificar intención de conexión pero no registrar si no hay id
                await Clients.Caller.SendAsync("UserConnected", (string?)null);
                return;
            }

            await Clients.Caller.SendAsync("UserConnected", userId);
            Users.TryAdd(Context.ConnectionId, userId);

            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "online";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                //await Clients.All.SendAsync("Users", user);

                // Si el usuario tiene bloqueos guardados en la base de datos, inicializamos el conjunto local
                if (user.BlockedUsers is not null && user.BlockedUsers.Length > 0)
                {
                    var blockedSet = _blockedUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
                    foreach (var blockedId in user.BlockedUsers)
                    {
                        if (!string.IsNullOrWhiteSpace(blockedId))
                        {
                            blockedSet.TryAdd(blockedId, 0);
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

            if (string.IsNullOrWhiteSpace(userId))
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
            var connectionId = Users.FirstOrDefault(p => p.Value == toUserId).Key;
            if (!string.IsNullOrEmpty(connectionId))
            {
                await Clients.Client(connectionId).SendAsync("UserStatus", userId, status);
            }
        }

        public async Task MarkMessageAsReceived(string messageId, string receiverId)
        {
            // Busca la conexión del usuario receptor
            var receiverConnectionId = Users.FirstOrDefault(p => p.Value == receiverId).Key;

            if (string.IsNullOrEmpty(receiverConnectionId))
                return;

            // Envía el evento al usuario específico
            await Clients.Client(receiverConnectionId).SendAsync("MessageReceived", messageId);
        }












        // Método para bloquear a un usuario (validado y thread-safe)
        public async Task BlockUser(string userIdToBlock)
        {
            if (string.IsNullOrWhiteSpace(userIdToBlock))
            {
                await Clients.Caller.SendAsync("UserBlockError", "invalid_user_id");
                return;
            }

            // Obtén el ID del usuario actual desde el mapa de conexiones; si no existe, usa UserIdentifier como fallback
            var currentUserId = Users.TryGetValue(Context.ConnectionId, out var uid) ? uid : Context.UserIdentifier;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                await Clients.Caller.SendAsync("UserBlockError", "current_user_not_found");
                return;
            }

            if (currentUserId == userIdToBlock)
            {
                await Clients.Caller.SendAsync("UserBlockError", "cannot_block_self");
                return;
            }

            var blockedSet = _blockedUsers.GetOrAdd(currentUserId, _ => new ConcurrentDictionary<string, byte>());

            // Evita duplicados
            if (!blockedSet.TryAdd(userIdToBlock, 0))
            {
                await Clients.Caller.SendAsync("UserBlocked", userIdToBlock, "already_blocked");
                return;
            }

            // Intentar localizar la conexión del usuario afectado y actualizar chatmap por si escribe
            var targetConnectionId = Users.FirstOrDefault(p => p.Value == userIdToBlock).Key;
            if (!string.IsNullOrWhiteSpace(targetConnectionId))
            {
                await Clients.Client(targetConnectionId).SendAsync("UserBlocked", currentUserId);
            }
        }

        // Método para verificar si un usuario está bloqueado por otro
        public async Task IsUserBlocked(string senderId, string? receiverId = null)
        {
            if (string.IsNullOrWhiteSpace(senderId))
            {
                await Clients.Caller.SendAsync("MessageBlocked", false);
                return;
            }

            var targetReceiverId = receiverId;
            if (string.IsNullOrWhiteSpace(targetReceiverId))
            {
                targetReceiverId = Users.TryGetValue(Context.ConnectionId, out var uid) ? uid : Context.UserIdentifier;
                if (string.IsNullOrWhiteSpace(targetReceiverId))
                {
                    await Clients.Caller.SendAsync("MessageBlocked", false);
                    return;
                }
            }

            if (_blockedUsers.TryGetValue(targetReceiverId, out var blockedSet) && blockedSet.ContainsKey(senderId))
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
            if (string.IsNullOrWhiteSpace(userIdToUnblock))
            {
                await Clients.Caller.SendAsync("UserUnblockError", "invalid_user_id");
                return;
            }

            var currentUserId = Users.TryGetValue(Context.ConnectionId, out var uid) ? uid : Context.UserIdentifier;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                await Clients.Caller.SendAsync("UserUnblockError", "current_user_not_found");
                return;
            }

            if (_blockedUsers.TryGetValue(currentUserId, out var blockedSet))
            {
                blockedSet.TryRemove(userIdToUnblock, out _);
                if (blockedSet.IsEmpty)
                {
                    _blockedUsers.TryRemove(currentUserId, out _);
                }
            }

            // Intentar localizar la conexión del usuario afectado y actualizar chatmap por si escribe
            var targetConnectionId = Users.FirstOrDefault(p => p.Value == userIdToUnblock).Key;
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