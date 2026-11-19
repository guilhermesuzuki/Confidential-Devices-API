using Devices.Models;
using Devices.Models.Data;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devices.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController(
        IDbContextFactory<MyDbContext> factory,
        IValidator<int> delete,
        [FromKeyedServices("insert")] IValidator<Device> insert,
        [FromKeyedServices("update")] IValidator<Device> update,
        TimeProvider timeProvider
        ) : Controller
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewDeviceAsync([FromBody] Device device, CancellationToken cancellation)
        {
            var validationResult = await insert.ValidateAsync(device, cancellation);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            device.CreationTime = timeProvider.GetUtcNow();

            using var db = await factory.CreateDbContextAsync(cancellation);
            db.Devices.Add(device);
            await db.SaveChangesAsync(cancellation);
            return Ok(device);
        }
        
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDeviceAsync([FromBody] Device device, CancellationToken cancellation)
        {
            var validationResult = await update.ValidateAsync(device, cancellation);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }
            using var db = await factory.CreateDbContextAsync(cancellation);
            db.Devices.Update(device);
            await db.SaveChangesAsync(cancellation);
            return Ok(device);
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
    }
}
