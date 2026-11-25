using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UptimeMonitor.Api.Contracts;
using UptimeMonitor.Api.Models;

namespace UptimeMonitor.Api.Tests;

public sealed class MonitorEndpointTests : IClassFixture<MonitorApiFactory>
{
  private readonly HttpClient _client;

  public MonitorEndpointTests(MonitorApiFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task CreateMonitor_ReturnsCreateMonitor()
  {
    var request = new CreateMonitorRequest(
        Name: "Example API",
        Url: "https://example.com/health",
        IntervalSeconds: 60,
        TimeoutSeconds: 5,
        ExpectedStatusCode: 200
    );

    var response = await _client.PostAsJsonAsync("/api/v1/monitors", request);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var monitor = await response.Content.ReadFromJsonAsync<MonitoredEndpoint>();

    Assert.NotNull(monitor);
    Assert.NotEqual(Guid.Empty, monitor.Id);
    Assert.Equal("Example API", monitor.Name);
    Assert.Equal("https://example.com/health", monitor.Url);
    Assert.Equal("pending", monitor.Status);
    Assert.True(monitor.Enabled);
    Assert.Null(monitor.LastCheckedAt);
  }

  [Fact]
  public async Task GetMonitor_ReturnsPreviouslyCreatedMonitor()
  {
    var request = new CreateMonitorRequest(
        Name: "Portfolio API",
        Url: "https://portfolio.example.com/health",
        IntervalSeconds: 30,
        TimeoutSeconds: 5,
        ExpectedStatusCode: 200);

    var createResponse = await _client.PostAsJsonAsync("/api/v1/monitors", request);
    var createdMonitor = await createResponse.Content.ReadFromJsonAsync<MonitoredEndpoint>();

    Assert.NotNull(createdMonitor);

    var getResponse = await _client.GetAsync($"/api/v1/monitors/{createdMonitor.Id}");

    Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

    var returnedMonitor = await getResponse.Content.ReadFromJsonAsync<MonitoredEndpoint>();

    Assert.NotNull(returnedMonitor);
    Assert.Equal(createdMonitor.Id, returnedMonitor.Id);
    Assert.Equal("Portfolio API", returnedMonitor.Name);
  }

  [Fact]
  public async Task GetAllMonitors_ReturnsCreatedMonitors()
  {
    var request = new CreateMonitorRequest(
        Name: "List Test API",
        Url: "https://list.example.com/health",
        IntervalSeconds: 60,
        TimeoutSeconds: 5,
        ExpectedStatusCode: 200);

    var createResponse = await _client.PostAsJsonAsync("/api/v1/monitors", request);

    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var response = await _client.GetAsync("/api/v1/monitors");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var monitors = await response.Content.ReadFromJsonAsync<List<MonitoredEndpoint>>();

    Assert.NotNull(monitors);
    Assert.Contains(monitors, monitor => monitor.Name == "List Test API");
  }

  [Fact]
  public async Task CreateMonitor_WithInvalidUrl_ReturnsBadRequest()
  {
    var request = new CreateMonitorRequest(
        Name: "Invalid Monitor",
        Url: "not-a-valid-url",
        IntervalSeconds: 60,
        TimeoutSeconds: 5,
        ExpectedStatusCode: 200);

    var response = await _client.PostAsJsonAsync("/api/v1/monitors", request);
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateMonitor_WithTimeLongerThanInterval_ReturnsBadRequest()
  {
    var request = new CreateMonitorRequest(
        Name: "Invalid Timeout",
        Url: "https://example.com",
        IntervalSeconds: 10,
        TimeoutSeconds: 10,
        ExpectedStatusCode: 200);

    var response = await _client.PostAsJsonAsync("/api/v1/monitors", request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task GetMissingMonitor_returnsNotFound()
  {
    var response = await _client.GetAsync($"/api/v1/monitors/{Guid.NewGuid()}");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task UpdateMonitor_ReturnsUpdatedMonitor()
  {
    var createRequest = new CreateMonitorRequest(
        Name: "Original API",
        Url: "https://original.example.com/health",
        IntervalSeconds: 60,
        TimeoutSeconds: 5,
        ExpectedStatusCode: 200);

    var createResponse = await _client.PostAsJsonAsync("/api/v1/monitors", createRequest);

    var createdMonitor = await createResponse.Content.ReadFromJsonAsync<MonitoredEndpoint>();

    Assert.NotNull(createdMonitor);

    var updateRequest = new UpdateMonitorRequest(
        Name: "Updated API",
        Url: "https://updated.example.com/health",
        IntervalSeconds: 30,
        TimeoutSeconds: 4,
        ExpectedStatusCode: 204,
        Enabled: false);

    var updateResponse = await _client.PutAsJsonAsync($"/api/v1/monitors/{createdMonitor.Id}", updateRequest);
    Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

    var updatedMonitor = await updateResponse.Content.ReadFromJsonAsync<MonitoredEndpoint>();
    Assert.NotNull(updatedMonitor);
    Assert.Equal(createdMonitor.Id, updatedMonitor.Id);
    Assert.Equal("Updated API", updatedMonitor.Name);
    Assert.Equal("https://updated.example.com/health", updatedMonitor.Url);
    Assert.Equal(30, updatedMonitor.IntervalSeconds);
    Assert.Equal(4, updatedMonitor.TimeoutSeconds);
    Assert.Equal(204, updatedMonitor.ExpectedStatusCode);
    Assert.False(updatedMonitor.Enabled);
  }

  [Fact]
  public async Task UpdateMissingMonitor_returnsNotFound()
  {
    var request = new UpdateMonitorRequest(
      Name: "Missing API",
      Url: "https://missing.example.com",
      IntervalSeconds: 60,
      TimeoutSeconds: 5,
      ExpectedStatusCode: 200,
      Enabled: true
    );

    var response = await _client.PutAsJsonAsync($"/api/v1/monitors/{Guid.NewGuid()}", request);
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task DeleteMissingMonitor_ReturnsNotFound()
  {
    var response = await _client.DeleteAsync($"/api/v1/monitors/{Guid.NewGuid()}");
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}
