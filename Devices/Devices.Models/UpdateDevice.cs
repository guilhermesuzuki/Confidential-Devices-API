using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Models
{
    /// <summary>
    /// Update device DTO
    /// </summary>
    public class UpdateDevice
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public DeviceStates State { get; set; } = DeviceStates.Available;
    }
}
