using Core.Models;
using Sieve.Services;

namespace Core.Collections.Impl
{
    public class CustomFiltersCollection : ICustomFilters, ISieveCustomFilterMethods
    {
        // private readonly HomeArrayFilterDispatcher _homeDispatcher;
        // private readonly UserArrayFilterDispatcher _userDispatcher;

        // public CustomSieveFilters(
        //     HomeArrayFilterDispatcher homeDispatcher,
        //     UserArrayFilterDispatcher userDispatcher)
        // {
        //     _homeDispatcher = homeDispatcher;
        //     _userDispatcher = userDispatcher;
        // }

        // -------- HOME --------

        // public IQueryable<Home> LikedBy(
        //     IQueryable<Home> source,
        //     string op,
        //     string[] values)
        //     => _homeDispatcher.Apply("likedBy", source, op, values);

        // // -------- USER --------

        // public IQueryable<User> BlockedUser(
        //     IQueryable<User> source,
        //     string op,
        //     string[] values)
        //     => _userDispatcher.Apply("blockedUser", source, op, values);

        // public IQueryable<User> LikedHome(
        //     IQueryable<User> source,
        //     string op,
        //     string[] values)
        //     => _userDispatcher.Apply("likedHome", source, op, values);
    }
}