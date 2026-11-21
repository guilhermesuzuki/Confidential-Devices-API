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

            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.Where(x => x.Id == newDevice.Id);
            var device = await query.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            device.ShouldNotBeNull();
            device.Id.ShouldBe(newDevice.Id);
            device.Name.ShouldBe(newDevice.Name);
            device.Brand.ShouldBe(newDevice.Brand);
            device.State.ShouldBe(newDevice.State);
            device.CreationTime.ShouldBe(newDevice.CreationTime);
        }

        [Fact]
        public async Task Delete_WhenEverythingIsOkay_ShouldReturnSuccess()
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
            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var newDevice = System.Text.Json.JsonSerializer.Deserialize<Device>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            newDevice.ShouldNotBeNull();
            newDevice.Id.ShouldBeGreaterThan(0);
            var deleteResponse = await this._factory.DeleteAsync(newDevice.Id);
            deleteResponse.IsSuccessStatusCode.ShouldBeTrue();

            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.Where(x => x.Id == newDevice.Id);
            var device = await query.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            device.ShouldBeNull();
        }

        [Fact]
        public async Task AllDevices_WhenEverythingIsOkay_ShouldReturnAllDevices()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var newDevices = new List<Device>();

            for (int i = 0; i < 5; i++)
            {
                var insertDevice = new InsertDevice
                {
                    Brand = $"Test Brand {i}",
                    Name = $"Test Name {i}",
                    State = DeviceStates.Available
                };

                var response = await this._factory.CreateAsync(insertDevice);
                response.IsSuccessStatusCode.ShouldBeTrue();
                var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
                var newDevice = System.Text.Json.JsonSerializer.Deserialize<Device>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                newDevice.ShouldNotBeNull();
                newDevice.Id.ShouldBeGreaterThan(0);
                newDevice.Name.ShouldBe(insertDevice.Name);
                newDevice.Brand.ShouldBe(insertDevice.Brand);

                newDevices.Add(newDevice);
            }

            var allDevicesResponse = await this._factory.FetchAllAsync();
            allDevicesResponse.IsSuccessStatusCode.ShouldBeTrue();
            var allDevicesJson = await allDevicesResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var allDevices = System.Text.Json.JsonSerializer.Deserialize<List<Device>>(allDevicesJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            allDevices.ShouldNotBeNull();
            allDevices.Count.ShouldBe(newDevices.Count);

            foreach (var newDevice in newDevices)
            {
                var found = allDevices.FirstOrDefault(x => x.Id == newDevice.Id);

                found.ShouldNotBeNull();
                found.Name.ShouldBe(newDevice.Name);
                found.Brand.ShouldBe(newDevice.Brand);
                found.State.ShouldBe(newDevice.State);
                found.CreationTime.UtcDateTime.ShouldBe(newDevice.CreationTime.UtcDateTime);
            }
        }

        [Fact]
        public async Task FetchByBrand_WhenEverythingIsOkay_ShouldReturnEntities()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var newDevices = new List<Device>();

            for (int i = 0; i < 5; i++)
            {
                var insertDevice = new InsertDevice
                {
                    Brand = $"Test Brand {i}",
                    Name = $"Test Name {i}",
                    State = DeviceStates.Available
                };

                var response = await this._factory.CreateAsync(insertDevice);
                response.IsSuccessStatusCode.ShouldBeTrue();
                var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
                var newDevice = System.Text.Json.JsonSerializer.Deserialize<Device>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                newDevice.ShouldNotBeNull();
                newDevice.Id.ShouldBeGreaterThan(0);
                newDevice.Name.ShouldBe(insertDevice.Name);
                newDevice.Brand.ShouldBe(insertDevice.Brand);

                newDevices.Add(newDevice);

                var brand = await this._factory.FetchByBrandAsync(newDevices[i].Brand);
                brand.IsSuccessStatusCode.ShouldBeTrue();

                var brandJson = await brand.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
                var allDevicesFromBrand = System.Text.Json.JsonSerializer.Deserialize<List<Device>>(brandJson, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                allDevicesFromBrand.ShouldNotBeNull();
                allDevicesFromBrand.Count.ShouldBe(1);
                allDevicesFromBrand[0].Brand.ShouldBe(newDevices[i].Brand);
                allDevicesFromBrand[0].CreationTime.UtcDateTime.ShouldBe(newDevices[i].CreationTime.UtcDateTime);
                allDevicesFromBrand[0].Id.ShouldBe(newDevices[i].Id);
                allDevicesFromBrand[0].Name.ShouldBe(newDevices[i].Name);
                allDevicesFromBrand[0].State.ShouldBe(newDevices[i].State);
            }
        }
    }
}
