using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UptimeMonitor.Api.Models;

namespace UptimeMonitor.Api.Data.Configurations;

public sealed class MonitoredEndpointConfguration : IEntityTypeConfiguration<MonitoredEndpoint>
{
    public void Configure(EntityTypeBuilder<MonitoredEndpoint> builder)
    {
        builder.ToTable("monitors");
        builder.HasKey(monitor => monitor.Id);

        builder.Property(monitor => monitor.Id).HasColumnName("id");
        builder.Property(monitor => monitor.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(monitor => monitor.Url).HasColumnName("url").HasMaxLength(2048).IsRequired();
        builder.Property(monitor => monitor.IntervalSeconds).HasColumnName("interval_seconds").IsRequired();
        builder.Property(monitor => monitor.TimeoutSeconds).HasColumnName("timeout_seconds").IsRequired();
        builder.Property(monitor => monitor.ExpectedStatusCode).HasColumnName("expected_status_code").IsRequired();
        builder.Property(monitor => monitor.Enabled).HasColumnName("enabled").IsRequired();
        builder.Property(monitor => monitor.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(monitor => monitor.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(monitor => monitor.LastCheckedAt).HasColumnName("last_checked_at");

        builder.HasIndex(monitor => monitor.CreatedAt);
        builder.HasIndex(monitor => monitor.Enabled);
    }   
}