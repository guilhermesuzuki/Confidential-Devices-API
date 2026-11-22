using Devices.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.Tests
{
    public class CustomFixture : IAsyncLifetime, IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly CustomWebApplicationFactory _factory;

        protected IServiceScope _scope = null!;
        protected IDbContextFactory<MyDbContext> _dbContextFactory = null!;

        private IDbContextTransaction _transaction = null!;

        public CustomFixture()
        {
            _factory = new();
        }

        public async ValueTask InitializeAsync()
        {
            _scope = _factory.Services.CreateScope();
            _dbContextFactory = _scope.ServiceProvider.GetRequiredService<IDbContextFactory<MyDbContext>>();
        }

        public async ValueTask DisposeAsync()
        {
            _scope.Dispose();
        }
    }
}
