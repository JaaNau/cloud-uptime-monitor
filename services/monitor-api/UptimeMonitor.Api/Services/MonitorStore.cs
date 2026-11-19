using System.Collections.Concurrent;
using UptimeMonitor.Api.Models;

namespace UptimeMonitor.Api.Services;

public sealed class MonitorStore
{
    private readonly ConcurrentDictionary<Guid, MonitoredEndpoint> _monitors = new();

    public IReadOnlyCollection<MonitoredEndpoint> GetAll()
    {
        return _monitors.Values.OrderBy(monitor => monitor.CreatedAt).ToArray();
    }

    public MonitoredEndpoint? GetById(Guid id)
    {
        return _monitors.GetValueOrDefault(id);
    }

    public MonitoredEndpoint Add(MonitoredEndpoint monitor)
    {
        if (!_monitors.TryAdd(monitor.Id, monitor))
        {
            throw new InvalidOperationException(
                $"A monitor with ID {monitor.Id} already exists.");
        }
        return monitor;
    }
}