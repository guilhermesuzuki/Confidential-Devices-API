using Devices.Models.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Devices.Models.Validations
{
    /// <summary>
    /// Device update fluent validation
    /// </summary>
    public class DeviceUpdateValidation : FluentValidation.AbstractValidator<UpdateDevice>
    {
        public DeviceUpdateValidation(IDbContextFactory<MyDbContext> factory)
        {
            // I know there's a trade off by pulling the same row from the database twice here, but it's because of
            // the way the fluent validations work.

            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (device, cancellation) =>
                {
                    using var db = await factory.CreateDbContextAsync(cancellation);
                    var deviceInDb = await db.Devices.FirstOrDefaultAsync(x => x.Id == device.Id, cancellation);
                    return deviceInDb != null;
                })
                .WithMessage("Device cannot be updated because it does not exist in the database.")
                .MustAsync(async (device, cancellation) =>
                {
                    using var db = await factory.CreateDbContextAsync(cancellation);
                    var deviceInDb = await db.Devices.FirstOrDefaultAsync(x => x.Id == device.Id, cancellation);
                    if (deviceInDb!.State == DeviceStates.InUse)
                    {
                        if (device.Name != null && deviceInDb.Name != device.Name)
                        {
                            return false;
                        }
                        if (device.Brand != null && deviceInDb.Brand != device.Brand)
                        {
                            return false;
                        }
                    }
                    return true;
                })
                .WithMessage("Device cannot be updated because it is currently in use.");
        }
    }
}
