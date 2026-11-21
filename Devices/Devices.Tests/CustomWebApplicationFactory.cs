using Devices.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Tests
{
    /// <summary>
    /// Custom web application factory for integration tests
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public async Task<HttpResponseMessage> CreateAsync(InsertDevice device)
        {
            using var client = CreateClient();
            return await client.PostAsJsonAsync("/api/devices/create", device);
        }

        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            using var client = CreateClient();
            return await client.DeleteAsync($"/api/devices/{id}");
        }

        public async Task<HttpResponseMessage> FetchAllAsync()
        {
            using var client = CreateClient();
            return await client.GetAsync("/api/devices");
        }

        public async Task<HttpResponseMessage> FetchByBrandAsync(string brand)
        {
            using var client = CreateClient();
            return await client.GetAsync($"/api/devices/brand?brand={brand}");
        }

        public async Task<HttpResponseMessage> FetchByStateAsync(DeviceStates state)
        {
            using var client = CreateClient();
            return await client.GetAsync($"/api/devices/state?state={state}");
        }

        public async Task<HttpResponseMessage> FetchSingleDeviceAsync(int id)
        {
            using var client = CreateClient();
            return await client.GetAsync($"/api/devices/{id}");
        }

        public async Task<HttpResponseMessage> UpdateAsync(UpdateDevice device)
        {
            using var client = CreateClient();
            return await client.PutAsJsonAsync("/api/devices/update", device);
        }
    }
}
