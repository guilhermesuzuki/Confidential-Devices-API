using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Globalization;

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
            optionsBuilder.ConfigureWarnings(warnings => 
            {
                warnings.Ignore(RelationalEventId.AmbientTransactionWarning);
            });
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
