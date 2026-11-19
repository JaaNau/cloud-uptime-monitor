using System.Collections.Concurrent;
using UptimeMonitor.Api.Models;

namespace UptimeMonitor.Api.services;

public sealed class MonitorStore
{
    private readonly ConcurrentDictionary<Guid, Monitor> _monitors = new();

    public IReadOnlyCollection<Monitor> GetAll()
    {
        return _monitors.Values.OrderBy(monitor => monitor.CreatedAt).ToArray();
    }

    public Models.Monitor? GetById(Guid id)
    {
        return _monitors.GetValueOrDefault(id);
    }

    public Models.Monitor Add(Models.Monitor monitor)
    {
        if (!_monitors.TryAdd(monitor.Id, monitor))
        {
            throw new InvalidOperationException(
                $"A monitor with ID {monitor.Id} already exists.");
        }
        return monitor;
    }
}