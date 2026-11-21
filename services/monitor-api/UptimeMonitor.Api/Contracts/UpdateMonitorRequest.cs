namespace UptimeMonitor.Api.Contracts;

public sealed record  UpdateMonitorRequest(
    string? Name,
    string? Url,
    int IntervalSeconds,
    int TimeoutSeconds,
    int ExpectedStatusCode,
    bool Enabled
);