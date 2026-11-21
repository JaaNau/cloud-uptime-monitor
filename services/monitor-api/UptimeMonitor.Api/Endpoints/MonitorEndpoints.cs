using UptimeMonitor.Api.Contracts;
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

  private static IResult GetAllMonitors(MonitorStore store)
  {
    return Results.Ok(store.GetAll());
  }

  private static IResult GetMonitorById(Guid id, MonitorStore store)
  {
    var monitor = store.GetById(id);
    return monitor is null ? Results.NotFound() : Results.Ok(monitor);
  }

  private static IResult CreateMonitor(CreateMonitorRequest request, MonitorStore store)
  {
    var validationErrors = ValidateRequest(request.Name,
      request.Url,
      request.IntervalSeconds,
      request.TimeoutSeconds,
      request.ExpectedStatusCode);
    if (validationErrors.Count > 0)
    {
      return Results.ValidationProblem(validationErrors);
    }

    var monitor = new MonitoredEndpoint(
        Id: Guid.NewGuid(),
        Name: request.Name!.Trim(),
        Url: request.Url!.Trim(),
        IntervalSeconds: request.IntervalSeconds,
        TimeoutSeconds: request.TimeoutSeconds,
        ExpectedStatusCode: request.ExpectedStatusCode,
        Enabled: true,
        Status: "pending",
        CreatedAt: DateTimeOffset.UtcNow,
        LastCheckedAt: null);

    store.Add(monitor);

    return Results.Created($"/api/v1/monitors/{monitor.Id}", monitor);
  }

  private static IResult UpdateMonitor(Guid id, UpdateMonitorRequest request, MonitorStore store)
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

    var updatedMonitor = store.Update(
      id,
      existingMonitor => existingMonitor with
      {
        Name = request.Name!.Trim(),
        Url = request.Url!.Trim(),
        IntervalSeconds = request.IntervalSeconds,
        TimeoutSeconds = request.TimeoutSeconds,
        ExpectedStatusCode = request.ExpectedStatusCode,
        Enabled = request.Enabled
      });

    return updatedMonitor is null ? Results.NotFound() : Results.Ok(updatedMonitor);
  }

  private static IResult DeleteMonitor(Guid id, MonitorStore store)
  {
    return store.Remove(id) ? Results.NoContent() : Results.NotFound();
  }

  private static Dictionary<string, string[]> ValidateRequest(string? name,
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

