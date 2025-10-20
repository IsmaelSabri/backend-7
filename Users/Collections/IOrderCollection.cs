using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Models;
namespace Users.Collections
{
    public interface IOrderCollection
    {
                Task Add(Order order);

    }
}