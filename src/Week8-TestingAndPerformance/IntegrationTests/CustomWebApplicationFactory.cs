using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SampleApi.Data;

namespace IntegrationTests;

/// <summary>
/// Hosts <c>SampleApi</c> in-process for HTTP-level tests. Each factory instance gets its own
/// uniquely named in-memory database so test classes never leak state into one another.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"IntegrationTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ProductDbContext>>();
            services.AddDbContext<ProductDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
