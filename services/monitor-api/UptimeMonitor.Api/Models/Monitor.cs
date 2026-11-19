namespace UptimeMonitor.Api.Models;

public sealed record Monitor(
    Guid Id,
    string Name,
    string Url,
    int IntervalSeconds,
    int TimeoutSeconds,
    int ExpectedStatusCode,
    bool Enabled,
    string Status, 
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);