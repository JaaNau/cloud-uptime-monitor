namespace UptimeMonitor.Api.Contracts;

public sealed record CreateMonitorRequest(
    string? Name,
    string? Url,
    int IntervalSeconds = 60,
    int TimeoutSeconds = 5,
    int ExpectedStatusCode = 200
);