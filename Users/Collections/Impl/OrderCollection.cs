using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Data;
using Users.Models;

namespace Users.Collections.Impl
{
    public class OrderCollection(UserDb db) : IOrderCollection
    {
        private readonly UserDb db = db;
        public async Task Add(Order order)
        {
            db.Orders.Add(order);
            await db.SaveChangesAsync();
        }
    }
}