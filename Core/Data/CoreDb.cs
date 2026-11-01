using System.Text.Json;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        public DbSet<ExtraContent> ExtraContents { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
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
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<Garage> Garages { get; set; }
        public DbSet<TrackEvent> TrackEvents { get; set; }
        public DbSet<AnalyticsIdentity> AnalyticsIdentities { get; set; }
        public DbSet<UserRefreshSession> UserRefreshSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<TrackEvent>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<TrackEvent>()
            .ToTable("TrackEvent")
            .Property(e => e.Metadata)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? new Dictionary<string, object>()
                    : JsonSerializer.Deserialize<Dictionary<string, object>>(v)
                        ?? new Dictionary<string, object>())
            .Metadata.SetValueComparer(
                new ValueComparer<Dictionary<string, object>>(
                    (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null)
                        == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
                    v => v == null
                        ? 0
                        : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
                    v => v == null
                        ? new Dictionary<string, object>()
                        : JsonSerializer.Deserialize<Dictionary<string, object>>(
                            JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                            (JsonSerializerOptions?)null
                        ) ?? new Dictionary<string, object>()
                )
            );

            builder.Entity<AnalyticsIdentity>()
                        .ToTable("AnalyticsIdentity")
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<UserRefreshSession>()
            .ToTable("UserRefreshSession")
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Chat>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Extra>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Cart>()
            .HasIndex(c => c.UsuarioId)
            .IsUnique();

            builder.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
            .Property(i => i.PrecioUnitario)
            .HasPrecision(18, 2);

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

            builder.Entity<PasswordResetOtp>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<PasswordResetToken>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<Garage>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

            // NUEVA CONFIGURACIÓN: Relación Uno a Muchos (1:N)
            builder.Entity<Garage>()
                .HasOne(g => g.Home)             // Un Garaje tiene UNA vivienda
                .WithMany(h => h.Garages)        // Una Vivienda tiene MUCHOS garajes
                .HasForeignKey(g => g.HomeId)    // La clave foránea está en Garage
                .OnDelete(DeleteBehavior.Cascade); // Si se borra la vivienda, se borran sus garajes

            builder.Entity<Flat>().ToTable("Flat");
            builder.Entity<House>().ToTable("House");
            builder.Entity<Room>().ToTable("Room");
            builder.Entity<HolidayRent>().ToTable("HolidayRent");
            builder.Entity<NewProject>().ToTable("NewProject");
            builder.Entity<Other>().ToTable("Other");
            builder.Entity<Extra>().ToTable("Extra");
            builder.Entity<ExtraContent>().ToTable("ExtraContent");
            builder.Entity<Garage>().ToTable("Garages");
            // Mapeo explícito para CART (Forzando mayúsculas y nombres exactos de columnas)
            builder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart"); //  c.Id).HasColumnName("Id");
                entity.Property(c => c.UsuarioId).HasColumnName("UsuarioId");
                entity.Property(c => c.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(c => c.LastUpdate).HasColumnName("LastUpdate");

                entity.HasIndex(c => c.UsuarioId).IsUnique();

                entity.HasMany(c => c.Items)
                      .WithOne()
                      .HasForeignKey(i => i.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Mapeo explícito para CARTITEM (Para que coincida con tu tabla en Postgres)
            builder.Entity<CartItem>(entity =>
            {
                // Si tu tabla en postgres la creaste con minúscula "id", ponla así aquí.
                // Si la creaste con "CartItem" en la base de datos, pon "CartItem".
                entity.ToTable("CartItem");

                entity.Property(i => i.Id).HasColumnName("id");
                entity.Property(i => i.CartId).HasColumnName("cartId");
                entity.Property(i => i.ExtraId).HasColumnName("extraId");
                entity.Property(i => i.ViviendaId).HasColumnName("viviendaId");
                entity.Property(i => i.NombreExtra).HasColumnName("nombreExtra");
                entity.Property(i => i.PrecioUnitario).HasColumnName("precioUnitario").HasPrecision(18, 2);
                entity.Property(i => i.Cantidad).HasColumnName("cantidad");
            });

            builder.Entity<Transaccion>()
                .HasIndex(x => x.StripePaymentIntentId)
                .IsUnique();

            builder.Entity<Transaccion>()
                .HasMany(x => x.Lineas)
                .WithOne()
                .HasForeignKey(x => x.TransaccionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}