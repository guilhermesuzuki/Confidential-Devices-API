using Devices.Models;
using Microsoft.AspNetCore.Mvc;
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
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);

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
        public async Task CreateDevice_WhenBrandIsEmpty_ShouldReturnError()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var insertDevice = new InsertDevice
            {
                Brand = string.Empty,
                Name = "Test Name",
                State = DeviceStates.Available
            };

            var response = await this._factory.CreateAsync(insertDevice);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Count.ShouldBe(1);
            errors[0].ShouldBe("Device brand cannot be empty.");

            var allDevicesInDatabase = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);

            allDevicesInDatabase.ShouldNotBeNull();
            allDevicesInDatabase.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateDevice_WhenBrandIsNull_ShouldReturnError()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var insertDevice = new InsertDevice
            {
                Brand = null!,
                Name = "Test Name",
                State = DeviceStates.Available
            };

            var response = await this._factory.CreateAsync(insertDevice);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<ValidationProblemDetails>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Status.ShouldBe(400);
            errors.Title.ShouldBe("One or more validation errors occurred.");
            errors.Errors.Count.ShouldBe(1);
            errors.Errors.First().Value.Count().ShouldBe(1);
            errors.Errors.First().Value.First().ShouldBe("The Brand field is required.");

            var allDevicesInDatabase = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);

            allDevicesInDatabase.ShouldNotBeNull();
            allDevicesInDatabase.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateDevice_WhenBrandIsTooLarge_ShouldReturnError()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var insertDevice = new InsertDevice
            {
                Brand = new string('A', 256),
                Name = "Test Name",
                State = DeviceStates.Available
            };

            var response = await this._factory.CreateAsync(insertDevice);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Count.ShouldBe(1);
            errors[0].ShouldBe("Device brand cannot exceed 64 characters.");

            var allDevicesInDatabase = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);

            allDevicesInDatabase.ShouldNotBeNull();
            allDevicesInDatabase.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateDevice_WhenNameIsEmpty_ShouldReturnError()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var insertDevice = new InsertDevice
            {
                Brand = "Test Brand",
                Name = string.Empty,
                State = DeviceStates.Available
            };

            var response = await this._factory.CreateAsync(insertDevice);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Count.ShouldBe(1);
            errors[0].ShouldBe("Device name cannot be empty.");

            var allDevicesInDatabase = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);

            allDevicesInDatabase.ShouldNotBeNull();
            allDevicesInDatabase.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateDevice_WhenNameIsNull_ShouldReturnError()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var insertDevice = new InsertDevice
            {
                Brand = "Test Brand",
                Name = null!,
                State = DeviceStates.Available
            };

            var response = await this._factory.CreateAsync(insertDevice);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<ValidationProblemDetails>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Status.ShouldBe(400);
            errors.Title.ShouldBe("One or more validation errors occurred.");
            errors.Errors.Count.ShouldBe(1);
            errors.Errors.First().Value.Count().ShouldBe(1);
            errors.Errors.First().Value.First().ShouldBe("The Name field is required.");

            var allDevicesInDatabase = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);

            allDevicesInDatabase.ShouldNotBeNull();
            allDevicesInDatabase.Count.ShouldBe(0);
        }

        [Fact]
        public async Task CreateDevice_WhenNameIsTooLarge_ShouldReturnError()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var insertDevice = new InsertDevice
            {
                Brand = "Test Brand",
                Name = new string('A', 256),
                State = DeviceStates.Available
            };

            var response = await this._factory.CreateAsync(insertDevice);

            response.IsSuccessStatusCode.ShouldBeFalse();
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Count.ShouldBe(1);
            errors[0].ShouldBe("Device name cannot exceed 64 characters.");

            var allDevicesInDatabase = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);

            allDevicesInDatabase.ShouldNotBeNull();
            allDevicesInDatabase.Count.ShouldBe(0);
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
        public async Task Delete_WhenDeviceIsInUse_ShouldReturnError()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var insertDevice = new InsertDevice
            {
                Brand = "Test Brand",
                Name = "Test Name",
                State = DeviceStates.InUse
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

            var deleteResponse = await this._factory.DeleteAsync(newDevice.Id);

            deleteResponse.IsSuccessStatusCode.ShouldBeFalse();
            deleteResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var deleteJson = await deleteResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(deleteJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Count.ShouldBe(1);
            errors[0].ShouldBe("Device cannot be deleted because it is currently in use.");

            var queryDeviceStillInDb = db.Devices.Where(x => x.Id == newDevice.Id);
            var deviceStillInDb = await queryDeviceStillInDb.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            deviceStillInDb.ShouldNotBeNull();
            deviceStillInDb.Id.ShouldBe(newDevice.Id);
            deviceStillInDb.Name.ShouldBe(newDevice.Name);
            deviceStillInDb.Brand.ShouldBe(newDevice.Brand);
            deviceStillInDb.State.ShouldBe(newDevice.State);
            deviceStillInDb.CreationTime.ShouldBe(newDevice.CreationTime);
        }

        [Fact]
        public async Task Delete_WhenDeviceDoesNotExist_ShouldReturnError()
        {
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);

            var deleteResponse = await this._factory.DeleteAsync(int.MaxValue);

            deleteResponse.IsSuccessStatusCode.ShouldBeFalse();
            deleteResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var deleteJson = await deleteResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(deleteJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Count.ShouldBe(1);
            errors[0].ShouldBe("Device cannot be deleted because it does not exist in the database.");

            var queryDeviceStillInDb = db.Devices.Where(x => x.Id == int.MaxValue);
            var deviceStillInDb = await queryDeviceStillInDb.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            deviceStillInDb.ShouldBeNull();
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
                newDevice.State.ShouldBe(insertDevice.State);

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

            var queryByBrand = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);

            for (int i = 0; i < newDevices.Count; i++)
            {
                var found = queryByBrand.FirstOrDefault(x => x.Id == newDevices[i].Id);

                found.ShouldNotBeNull();
                found.Name.ShouldBe(newDevices[i].Name);
                found.Brand.ShouldBe(newDevices[i].Brand);
                found.State.ShouldBe(newDevices[i].State);
                found.CreationTime.UtcDateTime.ShouldBe(newDevices[i].CreationTime.UtcDateTime);
            }
        }

        [Fact]
        public async Task FetchByState_WhenEverythingIsOkay_ShouldReturnEntities()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
            var query = db.Devices.AsQueryable();
            await query.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var newDevices = new List<Device>();

            for (int i = 0; i < 3; i++)
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
                newDevice.State.ShouldBe(insertDevice.State);

                newDevices.Add(newDevice);
            }

            for (int i = 0; i < 5; i++)
            {
                var insertDevice = new InsertDevice
                {
                    Brand = $"Test Brand {i}",
                    Name = $"Test Name {i}",
                    State = DeviceStates.Inactive
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
                newDevice.State.ShouldBe(insertDevice.State);

                newDevices.Add(newDevice);
            }

            for (int i = 0; i < 8; i++)
            {
                var insertDevice = new InsertDevice
                {
                    Brand = $"Test Brand {i}",
                    Name = $"Test Name {i}",
                    State = DeviceStates.InUse
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
                newDevice.State.ShouldBe(insertDevice.State);

                newDevices.Add(newDevice);
            }

            var availableResponse = await this._factory.FetchByStateAsync(DeviceStates.Available);

            availableResponse.IsSuccessStatusCode.ShouldBeTrue();
            var availableJson = await availableResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var availableDevices = System.Text.Json.JsonSerializer.Deserialize<List<Device>>(availableJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            availableDevices.ShouldNotBeNull();
            availableDevices.Count.ShouldBe(3);

            var inactiveResponse = await this._factory.FetchByStateAsync(DeviceStates.Inactive);
            var inactiveJson = await inactiveResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var inactiveDevices = System.Text.Json.JsonSerializer.Deserialize<List<Device>>(inactiveJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            inactiveDevices.ShouldNotBeNull();
            inactiveDevices.Count.ShouldBe(5);

            var inUseResponse = await this._factory.FetchByStateAsync(DeviceStates.InUse);
            var inUseJson = await inUseResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var inUseDevices = System.Text.Json.JsonSerializer.Deserialize<List<Device>>(inUseJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            inUseDevices.ShouldNotBeNull();
            inUseDevices.Count.ShouldBe(8);

            var queryAllDevices = await db.Devices.ToListAsync(TestContext.Current.CancellationToken);
            queryAllDevices.Count(x => x.State == DeviceStates.Available).ShouldBe(3);
            queryAllDevices.Count(x => x.State == DeviceStates.Inactive).ShouldBe(5);
            queryAllDevices.Count(x => x.State == DeviceStates.InUse).ShouldBe(8);
        }

        [Fact]
        public async Task FetchSingleDevice_WhenEverythingIsOkay_ShouldReturnSuccess()
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

            var fetchResponse = await this._factory.FetchSingleDeviceAsync(newDevice.Id);

            fetchResponse.IsSuccessStatusCode.ShouldBeTrue();
            var fetchJson = await fetchResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var fetchedDevice = System.Text.Json.JsonSerializer.Deserialize<Device>(fetchJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            fetchedDevice.ShouldNotBeNull();
            fetchedDevice.Id.ShouldBe(newDevice.Id);
            fetchedDevice.Name.ShouldBe(newDevice.Name);
            fetchedDevice.Brand.ShouldBe(newDevice.Brand);
            fetchedDevice.State.ShouldBe(newDevice.State);
            fetchedDevice.CreationTime.ShouldBe(newDevice.CreationTime);
        }

        [Fact]
        public async Task UpdateDevice_WhenEverythingIsOkay_ShouldReturnSuccess()
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

            var updateDevice = new UpdateDevice
            {
                Id = newDevice.Id,
                Brand = "Updated Brand",
                Name = "Updated Name",
                State = DeviceStates.InUse
            };

            var updateResponse = await this._factory.UpdateAsync(updateDevice);

            updateResponse.IsSuccessStatusCode.ShouldBeTrue();
            var updateJson = await updateResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var updatedDevice = System.Text.Json.JsonSerializer.Deserialize<Device>(updateJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            updatedDevice.ShouldNotBeNull();
            updatedDevice.Id.ShouldBe(newDevice.Id);
            updatedDevice.Brand.ShouldBe(updateDevice.Brand);
            updatedDevice.Name.ShouldBe(updateDevice.Name);
            updatedDevice.State.ShouldBe(updateDevice.State ?? DeviceStates.Available);
        }

        [Fact]
        public async Task UpdateDevice_WhenDeviceIsInUse_ShouldNotUpdateDevice()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var db = await this._dbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);

            var insertDevice = new InsertDevice
            {
                Brand = "Test Brand (InUse)",
                Name = "Test Name (InUse)",
                State = DeviceStates.InUse
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
            newDevice.Brand.ShouldBe(insertDevice.Brand);
            newDevice.Name.ShouldBe(insertDevice.Name);
            newDevice.State.ShouldBe(insertDevice.State);

            var query = db.Devices.Where(x => x.Id == newDevice.Id);
            var device = await query.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            device.ShouldNotBeNull();
            device.Id.ShouldBe(newDevice.Id);
            device.Name.ShouldBe(newDevice.Name);
            device.Brand.ShouldBe(newDevice.Brand);
            device.State.ShouldBe(newDevice.State);
            device.CreationTime.ShouldBe(newDevice.CreationTime);

            var updateDevice = new UpdateDevice
            {
                Id = newDevice.Id,
                Brand = "Updated Brand",
                Name = "Updated Name",
                State = DeviceStates.Available
            };

            var updateResponse = await this._factory.UpdateAsync(updateDevice);

            updateResponse.IsSuccessStatusCode.ShouldBeFalse();
            updateResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

            var updateJson = await updateResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var errors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(updateJson, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            errors.ShouldNotBeNull();
            errors.Count.ShouldBe(1);
            errors[0].ShouldBe("Device cannot be updated because it is currently in use.");

            var nonUpdatedDeviceQuery = db.Devices.Where(x => x.Id == newDevice.Id);
            var nonUpdatedDevice = await nonUpdatedDeviceQuery.FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            nonUpdatedDevice.ShouldNotBeNull();
            nonUpdatedDevice.Id.ShouldBe(newDevice.Id);
            nonUpdatedDevice.Brand.ShouldBe(newDevice.Brand);
            nonUpdatedDevice.CreationTime.UtcDateTime.ShouldBe(newDevice.CreationTime.UtcDateTime);
            nonUpdatedDevice.Name.ShouldBe(newDevice.Name);
            nonUpdatedDevice.State.ShouldBe(newDevice.State);
        }
    }
}
