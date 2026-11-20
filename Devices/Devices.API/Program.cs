using Devices.Models;
using Devices.Models.Data;
using Devices.Models.Validations;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<MyDbContext>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IValidator<int>, DeviceDeleteValidation>();
builder.Services.AddKeyedSingleton<IValidator<InsertDevice>, DeviceInsertValidation>("insert");
builder.Services.AddKeyedSingleton<IValidator<UpdateDevice>, DeviceUpdateValidation>("update");

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
