using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Models.Data
{
    public class MyDbContext : DbContext
    {
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
