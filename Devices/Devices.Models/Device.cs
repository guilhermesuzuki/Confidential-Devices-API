using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Devices.Models
{
    /// <summary>
    /// EF core Devices' table representation
    /// </summary>
    [Table("Devices")]
    public class Device
    {
        [StringLength(64)]
        public string Brand { get; set; } = string.Empty;

        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.MinValue;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(64)]
        public string Name { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceStates State { get; set; } = DeviceStates.Available;
    }
}
