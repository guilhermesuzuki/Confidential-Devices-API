using Devices.Models;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Transactions;

namespace Devices.Tests
{
    public class DevicesControllerTests : CustomFixture
    {
        [Fact]
        public async Task CreateDevice_WhenEverythingIsOkay_ShouldReturnSuccess()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var insertDevice = new InsertDevice
            {
                Brand = "Test Brand",
                Name = "Test Name",
                State = DeviceStates.Available
            };

            var response = await this._factory.CreateAsync(insertDevice);

            response.IsSuccessStatusCode.ShouldBeTrue();
            var json  = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var newDevice = System.Text.Json.JsonSerializer.Deserialize<Device>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            newDevice.ShouldNotBeNull();
            newDevice.Id.ShouldBeGreaterThan(0);
            newDevice.Brand.ShouldBe(insertDevice.Brand);
            newDevice.Name.ShouldBe(insertDevice.Name);
            newDevice.State.ShouldBe(insertDevice.State);

            var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.Where(x => x.Id == newDevice.Id);
            var device = await query.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            device.ShouldNotBeNull();
            device.Id.ShouldBe(device.Id);
            device.Name.ShouldBe(device.Name);
            device.Brand.ShouldBe(device.Brand);
            device.State.ShouldBe(device.State);
            device.CreationTime.ShouldBe(device.CreationTime);
        }
    }
}
