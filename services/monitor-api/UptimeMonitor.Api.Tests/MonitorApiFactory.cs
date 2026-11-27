using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UptimeMonitor.Api.Data;

namespace UptimeMonitor.Api.Tests;

public sealed class MonitorApiFactory : WebApplicationFactory<Program>
{
  private readonly string _databaseName =
      $"uptime-monitor-tests-{Guid.NewGuid()}";

  protected override void ConfigureWebHost(
      IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureServices(services =>
    {
      services.RemoveAll<MonitorDbContext>();

      services.RemoveAll<
              DbContextOptions<MonitorDbContext>>();

      services.RemoveAll<
              IDbContextOptionsConfiguration<MonitorDbContext>>();

      services.AddDbContext<MonitorDbContext>(
              options =>
              {
                options.UseInMemoryDatabase(_databaseName);
              });

      using var serviceProvider =
              services.BuildServiceProvider();

      using var scope =
              serviceProvider.CreateScope();

      var database =
              scope.ServiceProvider
                  .GetRequiredService<MonitorDbContext>();

      database.Database.EnsureCreated();
    });
  }
}
