var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
    service = "uptime-monitor-api",
    status = "running"
}));

app.MapHealthChecks("/health");

app.Run();

public partial class Program;