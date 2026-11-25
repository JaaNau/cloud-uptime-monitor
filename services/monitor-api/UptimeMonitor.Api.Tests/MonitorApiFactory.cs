using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UptimeMonitor.Api.Data;

namespace UptimeMonitor.Api.Tests;

public sealed class MonitorApiFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureServices(services =>
    {
        services.RemoveAll<DbContextOptions<MonitorDbContext>>();
        services.RemoveAll<MonitorDbContext>();

        services.AddDbContext<MonitorDbContext>(
            options =>
            {
                options.UseInMemoryDatabase(
                     $"uptime-monitor-tests-{Guid.NewGuid()}");
                
            }
        );
    });
  }
}