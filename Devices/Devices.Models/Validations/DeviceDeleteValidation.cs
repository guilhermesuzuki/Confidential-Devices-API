using Devices.Models.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Devices.Models.Validations
{
    /// <summary>
    /// Device delete fluent validation
    /// </summary>
    public class DeviceDeleteValidation : FluentValidation.AbstractValidator<int>
    {
        public DeviceDeleteValidation(IDbContextFactory<MyDbContext> factory) : base()
        {
            // I know there's a trade off by pulling the same row from the database twice here, but it's because of
            // the way the fluent validations work.

            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (id, cancellation) =>
                {
                    using var db = await factory.CreateDbContextAsync(cancellation);
                    var deviceInDb = await db.Devices.FirstOrDefaultAsync(x => x.Id == id, cancellation);
                    return deviceInDb != null;
                })
                .WithMessage("Device cannot be deleted because it does not exist in the database.")
                .MustAsync(async (id, cancellation) =>
                {
                    using var db = await factory.CreateDbContextAsync(cancellation);
                    var deviceInDb = await db.Devices.FirstOrDefaultAsync(x => x.Id == id, cancellation);
                    return deviceInDb!.State != DeviceStates.InUse;
                })
                .WithMessage("Device cannot be deleted because it is currently in use.");
        }
    }
}
