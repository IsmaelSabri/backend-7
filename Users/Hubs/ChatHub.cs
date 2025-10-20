using Microsoft.AspNetCore.SignalR;
using Users.Models;
using Users.Data;
namespace Users.Hubs
{
    public sealed class ChatHub(UserDb context) : Hub
    {
        public static Dictionary<string, string> Users = [];
        public async Task Connect(string userId)
        {
            Users.Add(Context.ConnectionId, userId);
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "online";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Users.TryGetValue(Context.ConnectionId, out string userId);
            Users.Remove(Context.ConnectionId);
            User? user = await context.Users.FindAsync(userId);
            if (user is not null)
            {
                user.Status = "offline";
                user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                await context.SaveChangesAsync();

                await Clients.All.SendAsync("Users", user);
            }
        }
    }

}