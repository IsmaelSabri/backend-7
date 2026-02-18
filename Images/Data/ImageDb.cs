using Images.Models;
using Microsoft.EntityFrameworkCore;

namespace Images.Data
{
    public class ImageDb : DbContext
    {
        // TPT model
        public ImageDb(DbContextOptions<ImageDb> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Image>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
        }
    }
}