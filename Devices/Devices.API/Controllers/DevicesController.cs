using Devices.Models;
using Devices.Models.Data;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devices.API.Controllers
{
    [ApiController]
    [Produces("text/json")]
    [Route("api/[controller]")]
    public class DevicesController(
        IDbContextFactory<MyDbContext> factory,
        IValidator<int> delete,
        [FromKeyedServices("insert")] IValidator<InsertDevice> insert,
        [FromKeyedServices("update")] IValidator<UpdateDevice> update,
        TimeProvider timeProvider
        ) : Controller
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewDeviceAsync([FromBody] InsertDevice device, CancellationToken cancellation)
        {
            var validationResult = await insert.ValidateAsync(device, cancellation);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var newDevice = new Device
            {
                Name = device.Name,
                Brand = device.Brand,
                State = device.State,
                CreationTime = timeProvider.GetUtcNow(),
            };

            using var db = await factory.CreateDbContextAsync(cancellation);
            db.Devices.Add(newDevice);
            await db.SaveChangesAsync(cancellation);
            return Ok(newDevice);
        }
        
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDeviceAsync([FromBody] UpdateDevice device, CancellationToken cancellation)
        {
            var validationResult = await update.ValidateAsync(device, cancellation);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            using var db = await factory.CreateDbContextAsync(cancellation);
            var updateDevice = await db.Devices.AsTracking().FirstOrDefaultAsync(x => x.Id == device.Id, cancellation);

            updateDevice!.Name = device.Name ?? updateDevice.Name;
            updateDevice!.Brand = device.Brand ?? updateDevice.Brand;
            updateDevice!.State = device.State ?? updateDevice.State;

            db.Devices.Update(updateDevice);
            await db.SaveChangesAsync(cancellation);
            return Ok(updateDevice);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> FetchSingleDeviceAsync([FromRoute] int id, CancellationToken cancellation)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid device ID.");
            }

            using var db = await factory.CreateDbContextAsync(cancellation);
            var deviceInDb = await db.Devices.FirstOrDefaultAsync(x => x.Id == id, cancellation);

            if (deviceInDb == null)
            {
                return BadRequest("Device not found.");
            }

            return Ok(deviceInDb);
        }

        [HttpGet]
        public async Task<IActionResult> FetchAllDevicesAsync(CancellationToken cancellation)
        {
            using var db = await factory.CreateDbContextAsync(cancellation);
            var devicesInDb = await db.Devices.ToListAsync(cancellation);
            return Ok(devicesInDb);
        }

        [HttpGet("brand")]
        public async Task<IActionResult> FetchDevicesByBrandAsync([FromQuery] string brand, CancellationToken cancellation)
        {
            using var db = await factory.CreateDbContextAsync(cancellation);
            var devicesInDb = await db.Devices.Where(x => x.Brand == brand).ToListAsync(cancellation);
            return Ok(devicesInDb);
        }

        [HttpGet("state")]
        public async Task<IActionResult> FetchDevicesByStateAsync([FromQuery] DeviceStates state, CancellationToken cancellation)
        {
            using var db = await factory.CreateDbContextAsync(cancellation);
            var devicesInDb = await db.Devices.Where(x => x.State == state).ToListAsync(cancellation);
            return Ok(devicesInDb);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDeviceAsync([FromRoute] int id, CancellationToken cancellation)
        {
            var validationResult = await delete.ValidateAsync(id, cancellation);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }
            using var db = await factory.CreateDbContextAsync(cancellation);
            var deviceInDb = await db.Devices.FirstOrDefaultAsync(x => x.Id == id, cancellation);
            db.Devices.Remove(deviceInDb!);
            await db.SaveChangesAsync(cancellation);
            return Ok("Device deleted succesfully");
        }
    }
}
