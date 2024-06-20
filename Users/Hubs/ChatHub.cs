using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Collections.Impl;
using Microsoft.AspNetCore.SignalR;
using Users.Dto;
using Users.Collections;
namespace Users.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatCollection chatCollection = new ChatCollection();
        public ChatHub(IChatCollection chatService)
        {
            chatCollection = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Chat");
            await Clients.Caller.SendAsync("UserConnected");
        }

        private async Task DisplayOnlineUsers()
        {
            var onlineUsers = chatCollection.GetOnlineUsers();
            await Clients.Groups("Chat").SendAsync("OnlineUsers", onlineUsers);
        }

        public async Task ReceiveMessage(MessageDto message)
        {
            await Clients.Group("Chat").SendAsync("NewMessage", message);
        }

        public async Task CreatePrivateChat(MessageDto message)
        {
            if (message.From != null && message.To != null)
            {
                string privateGroupName = GetPrivateGroupName(message.From, message.To);
                await Groups.AddToGroupAsync(Context.ConnectionId, privateGroupName);
                var toConnectionId = chatCollection.GetConnectionIdByUser(message.To);
                await Groups.AddToGroupAsync(toConnectionId, privateGroupName);

                // opening private chatbox for the other end user
                await Clients.Client(toConnectionId).SendAsync("OpenPrivateChat", message);
            }
        }

        public async Task RecivePrivateMessage(MessageDto message)
        {
            if (message.From != null && message.To != null)
            {
                string privateGroupName = GetPrivateGroupName(message.From, message.To);
                Console.WriteLine("to: "+message.To + "from: " + message.From + " content:" + message.Content);
                await Clients.Group(privateGroupName).SendAsync("NewPrivateMessage", message);
            }
        }

        public async Task RemovePrivateChat(string from, string to)
        {
            string privateGroupName = GetPrivateGroupName(from, to);
            await Clients.Group(privateGroupName).SendAsync("CloseProivateChat");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, privateGroupName);
            var toConnectionId = chatCollection.GetConnectionIdByUser(to);
            await Groups.RemoveFromGroupAsync(toConnectionId, privateGroupName);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Chat");
            var user = chatCollection.GetUserByConnectionId(Context.ConnectionId);
            chatCollection.RemoveUserFromList(user);
            await DisplayOnlineUsers();

            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddUserConnectionId(string name)
        {
            chatCollection.AddUserConnectinId(name, Context.ConnectionId);
            await DisplayOnlineUsers();
        }


        private static string GetPrivateGroupName(string from, string to)
        {
            var stringCompare = string.CompareOrdinal(from, to) < 0;
            return stringCompare ? $"{from}-{to}" : $"{to}-{from}";
        }
    }
}