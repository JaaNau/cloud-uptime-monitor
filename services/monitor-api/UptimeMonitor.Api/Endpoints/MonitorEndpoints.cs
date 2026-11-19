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
        var validationErrors = ValidateRequest(request);
        if(validationErrors.Count > 0)
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

    private static Dictionary<string, string[]> ValidateRequest(CreateMonitorRequest request)
    {
        var errors = new Dictionary<string,string[]>();
        if(string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Name is required."];
        }
        else if (request.Name.Trim().Length > 100)
        {
            errors["name"] = ["Name must not exceed 100 characters."];
        }

        if(string.IsNullOrWhiteSpace(request.Url))
        {
            errors["url"] = ["URL is required."];
        }
        else if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            errors["url"] = ["URL must be a valid HTTP or HTTPS URL."];
        }
                if (request.IntervalSeconds is < 10 or > 3600)
        {
            errors["intervalSeconds"] =
                ["Interval must be between 10 and 3600 seconds."];
        }

        if (request.TimeoutSeconds is < 1 or > 60)
        {
            errors["timeoutSeconds"] =
                ["Timeout must be between 1 and 60 seconds."];
        }
        else if (request.TimeoutSeconds >= request.IntervalSeconds)
        {
            errors["timeoutSeconds"] =
                ["Timeout must be shorter than the check interval."];
        }

        if (request.ExpectedStatusCode is < 100 or > 599)
        {
            errors["expectedStatusCode"] =
                ["Expected status code must be between 100 and 599."];
        }

        return errors;
    }
}

