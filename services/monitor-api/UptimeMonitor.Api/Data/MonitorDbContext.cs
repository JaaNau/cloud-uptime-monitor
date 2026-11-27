using Microsoft.EntityFrameworkCore;
using UptimeMonitor.Api.Models;

namespace UptimeMonitor.Api.Data;

public sealed class MonitorDbContext(DbContextOptions<MonitorDbContext> options) : DbContext(options)
{
  public DbSet<MonitoredEndpoint> Monitors =>
    Set<MonitoredEndpoint>();

  public DbSet<CheckResult> CheckResults =>
    Set<CheckResult>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(MonitorDbContext).Assembly);
  }
}
