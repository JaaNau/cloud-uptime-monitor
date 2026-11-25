using Microsoft.EntityFrameworkCore;
using UptimeMonitor.Api.Data;
using UptimeMonitor.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<MonitorDbContext>((serviceProvider, options) =>
{
  var configuration = serviceProvider.GetRequiredService<IConfiguration>();
  var connectionString = configuration.GetConnectionString("MonitorDatabase") 
  ?? throw new InvalidOperationException("Connection string 'MonitorDatabase' is missing.");
  options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
  service = "uptime-monitor-api",
  status = "running"
}));

app.MapHealthChecks("/health");
app.MapMonitorEndpoints();

app.Run();

public partial class Program;
