using Microsoft.AspNetCore.SignalR;
using Users.Models;
using Users.Data;
using System.Collections.Concurrent;
namespace Users.Hubs
{
    public sealed class ChatHub(UserDb context) : Hub
    {
        public static ConcurrentDictionary<string, string> Users = new();
        private static readonly ConcurrentDictionary<string, List<string>> _blockedUsers = new();

        public async Task Connect(string userId)
        {
            await Clients.Caller.SendAsync("UserConnected", userId); // Notifica a todos los clientes
            Users.TryAdd(Context.ConnectionId, userId);
            User? user = await context.Users.FindAsync(userId);
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
            Users.TryRemove(Context.ConnectionId, out string userId);
            //await Clients.Caller.SendAsync("UserDisconnected", userId); // Notifica a todos los clientes
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "offline";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }
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












        // Método para bloquear a un usuario
        public async Task BlockUser(string userIdToBlock)
        {
            var currentUserId = Context.UserIdentifier; // Obtén el ID del usuario actual

            if (!_blockedUsers.ContainsKey(currentUserId))
                _blockedUsers[currentUserId] = new List<string>();

            _blockedUsers[currentUserId].Add(userIdToBlock);

            await Clients.Caller.SendAsync("UserBlocked", userIdToBlock);
        }

        // Método para verificar si un usuario está bloqueado
        public async Task IsUserBlocked(string senderId, string receiverId)
        {
            if (_blockedUsers.TryGetValue(receiverId, out var blockedList) && blockedList.Contains(senderId))
            {
                await Clients.Caller.SendAsync("MessageBlocked", true);
            }
            await Clients.Caller.SendAsync("MessageBlocked", false);
        }

        // Sobrescribe OnConnected para manejar conexiones
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }

}