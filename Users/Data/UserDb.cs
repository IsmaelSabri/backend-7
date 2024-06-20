using Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Homes.Data
{
    public class UserDb : DbContext
    {
        // TPT model
        public UserDb(DbContextOptions<UserDb> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<User> Users { get; set; }//=> Set<Home>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        }
    }
}