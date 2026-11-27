using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UptimeMonitor.Api.Models;

namespace UptimeMonitor.Api.Data.Configurations;

public sealed class CheckResultConfiguration : IEntityTypeConfiguration<CheckResult>
{
  public void Configure(EntityTypeBuilder<CheckResult> builder)
  {
    builder.ToTable("check_results");
    builder.HasKey(result => result.Id);
    builder.Property(result => result.Id)
        .HasColumnName("id");
    builder.Property(result => result.MonitorId)
        .HasColumnName("monitor_id")
        .IsRequired();

    builder.Property(result => result.Success)
    .HasColumnName("success")
    .IsRequired();

    builder.Property(result => result.StatusCode)
        .HasColumnName("status_code");

    builder.Property(result => result.StatusCode)
    .HasColumnName("response_time_ms")
    .IsRequired();

    builder.Property(result => result.ErrorType)
        .HasColumnName("error_type")
        .HasMaxLength(100);

    builder.Property(result => result.ErrorMessage)
        .HasColumnName("error_message")
        .HasMaxLength(1000);

    builder.Property(result => result.CheckedAt)
        .HasColumnName("checked_at")
        .IsRequired();

    builder.HasOne(result => result.Monitor)
    .WithMany(monitor => monitor.CheckResults)
    .HasForeignKey(result => result.MonitorId)
    .OnDelete(DeleteBehavior.Cascade);

    builder.HasIndex(result => new
    {
      result.MonitorId,
      result.CheckedAt
    });
  }
}
