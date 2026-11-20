using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Devices.Models
{
    /// <summary>
    /// Update device DTO
    /// </summary>
    public class UpdateDevice
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceStates? State { get; set; }
    }
}
