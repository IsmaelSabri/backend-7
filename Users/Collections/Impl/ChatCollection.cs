using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Users.Models;
using Users.Repositories;

namespace Users.Collections.Impl
{
    public class ChatCollection : IChatCollection
    {
        private static readonly Dictionary<string, string> ChatWindow = new();
        private readonly IUserCollection db = new UserCollection();

        public async Task FillDictionaryMethod()
        {
            var users = await db.GetAllUsers();
            foreach (var w in users)
            {
                if(!string.IsNullOrEmpty(w.UserId)){
                    ChatWindow.Add(w.UserId, null!);
                }
            }
        }
        /*public bool AddUserToList(string userToAdd)
        {
            lock (ChatWindow)
            {
                foreach (var user in ChatWindow)
                {
                    if (string.Equals(user.Key, userToAdd, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                ChatWindow.Add(userToAdd, null!);
                return true;
            }
        }*/

        public void AddUserConnectinId(string user, string connectionId)
        {
            lock (ChatWindow)
            {
                FillDictionaryMethod();
                Thread.Sleep(3000);
                if (ChatWindow.ContainsKey(user))
                {
                    ChatWindow[user] = connectionId;
                }
            }
        }

        public string GetUserByConnectionId(string connectionId)
        {
            lock (ChatWindow)
            {
                return ChatWindow.Where(x => x.Value == connectionId).Select(x => x.Key).FirstOrDefault();
            }
        }

        public string GetConnectionIdByUser(string user)
        {
            lock (ChatWindow)
            {
                return ChatWindow.Where(x => x.Key == user).Select(x => x.Value).FirstOrDefault();
            }
        }

        public void RemoveUserFromList(string user)
        {
            lock (ChatWindow)
            {
                ChatWindow.Remove(user);
            }
        }

        public string[] GetOnlineUsers()
        {
            lock (ChatWindow)
            {
                return ChatWindow.OrderBy(x => x.Key).Select(x => x.Key).ToArray();
            }
        }
    }
}