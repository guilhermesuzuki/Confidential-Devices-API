using System.Text.Json.Serialization;

namespace Devices.Models
{
    /// <summary>
    /// Update device DTO
    /// </summary>
    public class UpdateDevice
    {
        public string? Brand { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceStates? State { get; set; }
    }
}
