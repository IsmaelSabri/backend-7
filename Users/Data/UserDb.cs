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
        public DbSet<Extra> Extras { get; set; }
        public DbSet<ExtraContent> ExtraSubscriptions { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<LineaTransaccion> LineasTransaccion { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanSubscription> PlanSubscriptions { get; set; }
        public DbSet<StripeWebhookEvent> StripeWebhookEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Chat>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Extra>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<ExtraContent>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<StripeWebhookEvent>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<LineaTransaccion>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Plan>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<PlanSubscription>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            // builder.Entity<Transaccion>()
            // .HasMany(t => t.Lineas)
            // .WithOne()
            // .HasForeignKey(l => l.TransaccionId)
            // .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaccion>()
                .HasIndex(x => x.StripePaymentIntentId)
                .IsUnique();
        }
    }
}