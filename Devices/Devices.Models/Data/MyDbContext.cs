using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Models.Data
{
    /// <summary>
    /// Devices' database context
    /// </summary>
    public class MyDbContext : DbContext
    {
        /// <summary>
        /// Devices' table
        /// </summary>
        public DbSet<Device> Devices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite("Data Source=devices.db");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>().Property(x => x.State).HasMaxLength(32);
            modelBuilder.Entity<Device>().Property(x => x.State).HasConversion(
                x => x.ToString(),
                x => Enum.Parse<DeviceStates>(x)
                );
        }
    }
}
