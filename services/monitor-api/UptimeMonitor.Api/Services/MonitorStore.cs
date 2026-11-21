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

  public MonitoredEndpoint? Update(Guid id, Func<MonitoredEndpoint, MonitoredEndpoint> update)
  {
    while (_monitors.TryGetValue(id, out var existingMonitor))
    {
      var updatedMonitor = update(existingMonitor);
      if (_monitors.TryUpdate(id, updatedMonitor, existingMonitor))
      {
        return updatedMonitor;
      }
    }
    return null;
  }

  public bool Remove(Guid id)
  {
    return _monitors.TryRemove(id, out _);
  }
}
