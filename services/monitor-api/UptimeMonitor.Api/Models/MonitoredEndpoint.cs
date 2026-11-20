namespace UptimeMonitor.Api.Models;

public sealed record MonitoredEndpoint(
    Guid Id,
    string Name,
    string Url,
    int IntervalSeconds,
    int TimeoutSeconds,
    int ExpectedStatusCode,
    bool Enabled,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastCheckedAt
);
