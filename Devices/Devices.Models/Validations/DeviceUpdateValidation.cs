using Devices.Models.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Models.Validations
{
    public class DeviceUpdateValidation : FluentValidation.AbstractValidator<UpdateDevice>
    {
        public DeviceUpdateValidation(IDbContextFactory<MyDbContext> factory)
        {
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
