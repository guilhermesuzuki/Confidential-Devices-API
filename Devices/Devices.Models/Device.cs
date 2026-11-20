using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Devices.Models
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(64)]
        public string Name { get; set; } = string.Empty;

        [StringLength(64)]
        public string Brand { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceStates State { get; set; } = DeviceStates.Available;

        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.MinValue;
    }
}
