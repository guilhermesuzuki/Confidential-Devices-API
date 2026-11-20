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
        /// <summary>
        /// This endpoint creates a new device in a POST http method, validating the insert device body payload.
        /// </summary>
        /// <param name="device">information to create a new device</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns>Status 400 when something goes wrong, or 200 when everything goes fine</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// This endpoint deletes an existing device, validating the ID (if the device exists in the database).
        /// </summary>
        /// <param name="id">Device ID to delete</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// This endpoint fetches all devices from the database.
        /// </summary>
        /// <param name="cancellation">cancellation token</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FetchAllDevicesAsync(CancellationToken cancellation)
        {
            using var db = await factory.CreateDbContextAsync(cancellation);
            var devicesInDb = await db.Devices.ToListAsync(cancellation);
            return Ok(devicesInDb);
        }

        /// <summary>
        /// This endpoint fetches all devices by brand
        /// </summary>
        /// <param name="brand">brand name</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns></returns>
        [HttpGet("brand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FetchDevicesByBrandAsync([FromQuery] string brand, CancellationToken cancellation)
        {
            using var db = await factory.CreateDbContextAsync(cancellation);

            // point of improvement: the documentation doesn't say anything about this fetching endpoint being case insensitive, but I did it anyway, because it made sense.
            var query = from d in db.Devices.AsQueryable()
                        where EF.Functions.Like(d.Brand, brand)
                        select d;

            var devicesInDb = await query.ToListAsync(cancellation);
            return Ok(devicesInDb);
        }

        /// <summary>
        /// This endpoint fetches all devices by state
        /// </summary>
        /// <param name="state">state</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns></returns>
        [HttpGet("state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FetchDevicesByStateAsync([FromQuery] DeviceStates state, CancellationToken cancellation)
        {
            using var db = await factory.CreateDbContextAsync(cancellation);
            var devicesInDb = await db.Devices.Where(x => x.State == state).ToListAsync(cancellation);
            return Ok(devicesInDb);
        }

        /// <summary>
        /// This endpoint fetches a single device, given an ID.
        /// </summary>
        /// <param name="id">ID to retrieve</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// This endpoint fully/partially updates a device in the database, depending on the information present in the update device payload.
        /// </summary>
        /// <param name="device">update device payload</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    }
}
