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
    public class DeviceInsertValidation : AbstractValidator<InsertDevice>
    {
        public DeviceInsertValidation(IDbContextFactory<MyDbContext> factory) : base()
        {
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
