using Microsoft.EntityFrameworkCore;
using UptimeMonitor.Api.Contracts;
using UptimeMonitor.Api.Data;
using UptimeMonitor.Api.Models;
using UptimeMonitor.Api.Services;

namespace UptimeMonitor.Api.Endpoints;

public static class MonitorEndpoints
{
  public static IEndpointRouteBuilder MapMonitorEndpoints(this IEndpointRouteBuilder endpoints)
  {
    var group = endpoints.MapGroup("/api/v1/monitors")
        .WithTags("Monitors");

    group.MapGet("/", GetAllMonitors);
    group.MapGet("/{id:guid}", GetMonitorById);
    group.MapPost("/", CreateMonitor);
    group.MapPut("/{id:guid}", UpdateMonitor);
    group.MapDelete("/{id:guid}", DeleteMonitor);

    return endpoints;
  }

  private static async Task<IResult> GetAllMonitors(MonitorDbContext database, CancellationToken cancellationToken)
  {
    var monitors = await database.Monitors
    .AsNoTracking()
    .OrderBy(monitor => monitor.CreatedAt)
    .ToListAsync(cancellationToken);

    return Results.Ok(monitors);
  }

  private static async Task <IResult> GetMonitorById(Guid id, MonitorDbContext database, CancellationToken cancellationToken)
  {
    var monitor = await database.Monitors
    .AsNoTracking()
    .SingleOrDefaultAsync(monitor => monitor.Id == id, cancellationToken);
    return monitor is null ? Results.NotFound() : Results.Ok(monitor);
  }

  private static async Task<IResult> CreateMonitor(CreateMonitorRequest request, MonitorDbContext database,
        CancellationToken cancellationToken)
  {
    var validationErrors = ValidateRequest(
      request.Name,
      request.Url,
      request.IntervalSeconds,
      request.TimeoutSeconds,
      request.ExpectedStatusCode);
    if (validationErrors.Count > 0)
    {
      return Results.ValidationProblem(validationErrors);
    }

    var monitor = new MonitoredEndpoint
    {
        Id = Guid.NewGuid(),
        Name = request.Name!.Trim(),
        Url = request.Url!.Trim(),
        IntervalSeconds = request.IntervalSeconds,
        TimeoutSeconds = request.TimeoutSeconds,
        ExpectedStatusCode = request.ExpectedStatusCode,
        Enabled = true,
        Status = "pending",
        CreatedAt = DateTimeOffset.UtcNow,
        LastCheckedAt = null
    };

    await database.Monitors.AddAsync(monitor, cancellationToken);

    await database.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/v1/monitors/{monitor.Id}", monitor);
  }

  private static async Task<IResult> UpdateMonitor(Guid id,
      UpdateMonitorRequest request,
      MonitorDbContext database,
      CancellationToken cancellationToken)
  {
    var validationErrors = ValidateRequest(
      request.Name,
      request.Url,
      request.IntervalSeconds,
      request.TimeoutSeconds,
      request.ExpectedStatusCode);

    if (validationErrors.Count > 0)
    {
      return Results.ValidationProblem(validationErrors);
    }

    var monitor = await database.Monitors.SingleOrDefaultAsync(monitor => monitor.Id == id, cancellationToken);
    if (monitor is null)
    {
      return Results.NotFound();
    }
      
    monitor.Name = request.Name!.Trim();
    monitor.Url = request.Url!.Trim();
    monitor.IntervalSeconds = request.IntervalSeconds;
    monitor.TimeoutSeconds = request.TimeoutSeconds;
    monitor.ExpectedStatusCode = request.ExpectedStatusCode;
    monitor.Enabled = request.Enabled;
    
    await database.SaveChangesAsync(cancellationToken);

    return Results.Ok(monitor);
  }

  private static async Task<IResult> DeleteMonitor(Guid id, MonitorDbContext database,
        CancellationToken cancellationToken)
  {
    var monitor = await database.Monitors.SingleOrDefaultAsync(
    monitor => monitor.Id == id, cancellationToken);

    if (monitor is null)
    {
      return Results.NotFound();
    }

    database.Monitors.Remove(monitor);
    await database.SaveChangesAsync(cancellationToken);

    return Results.NoContent();
    }

  private static Dictionary<string, string[]> ValidateRequest(
    string? name,
    string? url,
    int intervalSeconds,
    int timeoutSeconds,
    int expectedStatusCode)
  {
    var errors = new Dictionary<string, string[]>();
    if (string.IsNullOrWhiteSpace(name))
    {
      errors["name"] = ["Name is required."];
    }
    else if (name.Trim().Length > 100)
    {
      errors["name"] = ["Name must not exceed 100 characters."];
    }

    if (string.IsNullOrWhiteSpace(url))
    {
      errors["url"] = ["URL is required."];
    }
    else if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
    {
      errors["url"] = ["URL must be a valid HTTP or HTTPS URL."];
    }
    if (intervalSeconds is < 10 or > 3600)
    {
      errors["intervalSeconds"] =
          ["Interval must be between 10 and 3600 seconds."];
    }

    if (timeoutSeconds is < 1 or > 60)
    {
      errors["timeoutSeconds"] =
          ["Timeout must be between 1 and 60 seconds."];
    }
    else if (timeoutSeconds >= intervalSeconds)
    {
      errors["timeoutSeconds"] =
          ["Timeout must be shorter than the check interval."];
    }

    if (expectedStatusCode is < 100 or > 599)
    {
      errors["expectedStatusCode"] =
          ["Expected status code must be between 100 and 599."];
    }

    return errors;
  }
}

