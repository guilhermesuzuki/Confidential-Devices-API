using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Devices.Models
{
    /// <summary>
    /// Insert device DTO
    /// </summary>
    public class InsertDevice
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceStates State { get; set; } = DeviceStates.Available;
    }
}
