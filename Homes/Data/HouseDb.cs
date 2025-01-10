using Homes.Models;
using Microsoft.EntityFrameworkCore;

namespace Homes.Data
{
    public class HouseDb : DbContext
    {
        // TPT model
        public HouseDb(DbContextOptions<HouseDb> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<Home> Homes { get; set; }//=> Set<Home>();
        public DbSet<Flat> Flats { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<HolidayRent> HolidayRents { get; set; }
        public DbSet<NewProject> NewProjects { get; set; }
        public DbSet<Other> Others { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flat>().ToTable("Flat");
            modelBuilder.Entity<House>().ToTable("House");
            modelBuilder.Entity<Room>().ToTable("Room");
            modelBuilder.Entity<HolidayRent>().ToTable("HolidayRent");
            modelBuilder.Entity<NewProject>().ToTable("NewProject");
            modelBuilder.Entity<Other>().ToTable("Other");
            //modelBuilder.Entity<Home>().UseTphMappingStrategy().Property(e=>e.Id).HasDefaultValue;
        }
    }
}