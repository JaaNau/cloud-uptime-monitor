namespace UptimeMonitor.Api.Models;

public sealed class CheckResult
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }

    public bool Success { get; set; }
    public int? StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? ErrorType { get; set; }
    public string? ErrorMessage {get; set; }
    public DateTimeOffset CheckedAt { get; set; }
    public MonitoredEndpoint Monitor { get; set; } = null!;
}