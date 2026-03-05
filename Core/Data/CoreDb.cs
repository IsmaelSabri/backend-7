using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class CoreDb : DbContext
    {
        // TPT model
        public CoreDb(DbContextOptions<CoreDb> options) : base(options)
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
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Home> Homes { get; set; }//=> Set<Home>();
        public DbSet<Flat> Flats { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<HolidayRent> HolidayRents { get; set; }
        public DbSet<NewProject> NewProjects { get; set; }
        public DbSet<Other> Others { get; set; }

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

            builder.Entity<Organization>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Home>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Flat>().ToTable("Flat");
            builder.Entity<House>().ToTable("House");
            builder.Entity<Room>().ToTable("Room");
            builder.Entity<HolidayRent>().ToTable("HolidayRent");
            builder.Entity<NewProject>().ToTable("NewProject");
            builder.Entity<Other>().ToTable("Other");

            builder.Entity<Transaccion>()
                .HasIndex(x => x.StripePaymentIntentId)
                .IsUnique();
        }
    }
}