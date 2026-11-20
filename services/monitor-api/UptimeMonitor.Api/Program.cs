using UptimeMonitor.Api.Endpoints;
using UptimeMonitor.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<MonitorStore>();

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