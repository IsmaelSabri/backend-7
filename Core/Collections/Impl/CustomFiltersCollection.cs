using Core.Collections.impl;
using Core.Collections.Impl;
using Core.Models;
using Sieve.Services;
using Stripe;

namespace Core.Collections.Impl
{
    public class CustomFiltersCollection : ICustomFilters, ISieveCustomFilterMethods
    {
        public class CustomSieveFilters : ISieveCustomFilterMethods
        {
            private readonly UserCollection _userCollection;
            private readonly HomeCollection _homeCollection;
            private readonly ChatsCollection _chatCollection;
            // private readonly PaymentCollection _paymentCollection;

            public CustomSieveFilters(
                UserCollection userCollection,
                HomeCollection homeCollection,
                ChatsCollection chatCollection)
                // PaymentCollection paymentCollection)
            {
                _userCollection = userCollection;
                _homeCollection = homeCollection;
                _chatCollection = chatCollection;
                // _paymentCollection = paymentCollection;
            }

            // Ejemplo de filtro custom
            public IQueryable<T> BlockedUsers<T>(IQueryable<T> source, string op, string[] values)
                where T : class
            {
                if (typeof(T) == typeof(User))
                {
                    // delegamos al servicio de User
                    var q = _userCollection.GetBlockYou(source as IQueryable<User>, op,values);
                    return (IQueryable<T>)q;
                }

                // si no aplica, devolvemos el source sin cambios
                return source;
            }

            public IQueryable<T> PriceGreaterThan<T>(IQueryable<T> source, string op, string[] values)
                where T : class
            {
                if (typeof(T) == typeof(Home))
                {
                    // var q = _homeCollection.ApplyPriceGreaterThanFilter(source as IQueryable<Home>, values);
                    // return (IQueryable<T>)q;
                }

                return source;
            }

            // puedes añadir más filtros de Chat, Payment, etc.
        }
    }
}