using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.Data
{
    public class EmailDbContext : DbContext
    {
        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        public DbSet<UsersContactMessage> UsersContactMessages => Set<UsersContactMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ContactMessage
            modelBuilder.Entity<ContactMessage>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ContactMessage>()
                .HasIndex(x => x.Email);

            modelBuilder.Entity<ContactMessage>()
                .HasIndex(x => x.Category);


            // UsersContactMessage
            modelBuilder.Entity<UsersContactMessage>()
                .ToTable("UsersContactMessages");

            modelBuilder.Entity<UsersContactMessage>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UsersContactMessage>()
                .HasIndex(x => x.UserId);

            modelBuilder.Entity<UsersContactMessage>()
                .HasIndex(x => x.FromEmail);

            modelBuilder.Entity<UsersContactMessage>()
                .HasIndex(x => x.ToEmail);

            modelBuilder.Entity<UsersContactMessage>()
                .HasIndex(x => x.CreatedAt);

            base.OnModelCreating(modelBuilder);
        }
    }
}