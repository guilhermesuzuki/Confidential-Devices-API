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
    public class DeviceInsertValidation : AbstractValidator<Device>
    {
        public DeviceInsertValidation(IDbContextFactory<MyDbContext> factory) : base()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .Must(x => x <= 0)
                .WithMessage("Invalid device ID.")
                .MustAsync(async (id, cancellation) =>
                {
                    using var db = await factory.CreateDbContextAsync(cancellation);
                    var deviceInDb = await db.Devices.FirstOrDefaultAsync(x => x.Id == id, cancellation);
                    return deviceInDb == null;
                })
                .WithMessage("Device cannot be inserted because the ID already exists in the database.")
                ;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Device name is required.")
                .NotEmpty().WithMessage("Device name cannot be empty.")
                .MaximumLength(64).WithMessage("Device name cannot exceed 64 characters.")
                ;

            RuleFor(x => x.Brand)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Device brand is required.")
                .NotEmpty().WithMessage("Device brand cannot be empty.")
                .MaximumLength(64).WithMessage("Device brand cannot exceed 64 characters.")
                ;
        }
    }
}
