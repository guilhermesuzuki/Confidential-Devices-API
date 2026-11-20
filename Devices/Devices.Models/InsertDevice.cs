using System.Text.Json.Serialization;

namespace Devices.Models
{
    /// <summary>
    /// Insert device DTO
    /// </summary>
    public class InsertDevice
    {
        public string Brand { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceStates State { get; set; } = DeviceStates.Available;
    }
}
