using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homes.Models;
using Microsoft.EntityFrameworkCore;

namespace Homes.Data
{
    public class HouseDb : DbContext
    {
        public HouseDb(DbContextOptions<HouseDb> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<Home> Homes { get; set; }//=> Set<Home>();
    }
}