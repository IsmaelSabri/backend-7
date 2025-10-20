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
        public DbSet<Order> Orders { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Chat>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Image>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<User>()
            .HasMany(e => e.Orders)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired(false);


        }
    }
}