using System.ComponentModel.DataAnnotations;

namespace Devices.Models
{
    public class Device
    {
        public int Id { get; set; }

        [StringLength(64)]
        public string Name { get; set; } = string.Empty;

        [StringLength(64)]
        public string Brand { get; set; } = string.Empty;

        public DeviceStates State { get; set; } = DeviceStates.Available;

        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.MinValue;
    }
}
