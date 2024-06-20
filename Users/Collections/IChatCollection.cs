using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.Collections
{
    public interface IChatCollection
    {
        //bool AddUserToList(string userToAdd);
        void AddUserConnectinId(string user, string connectionId);
        string GetUserByConnectionId(string connectionId);
        string GetConnectionIdByUser(string user);
        void RemoveUserFromList(string user);
        string[] GetOnlineUsers();
    }
}