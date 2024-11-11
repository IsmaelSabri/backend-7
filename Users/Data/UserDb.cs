using Users.Models;
using Microsoft.EntityFrameworkCore;

namespace Users.Data
{
    public class UserDb : DbContext
    {
        // TPT model
        public UserDb(DbContextOptions<UserDb> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<User> Users { get; set; }//=> Set<Home>();
        public DbSet<Chat> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Chat>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        }
    }
}