using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UptimeMonitor.Api.Data.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "monitors",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uuid", nullable: false),
            name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
            interval_seconds = table.Column<int>(type: "integer", nullable: false),
            timeout_seconds = table.Column<int>(type: "integer", nullable: false),
            expected_status_code = table.Column<int>(type: "integer", nullable: false),
            enabled = table.Column<bool>(type: "boolean", nullable: false),
            status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
            last_checked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_monitors", x => x.id);
          });

      migrationBuilder.CreateIndex(
          name: "IX_monitors_created_at",
          table: "monitors",
          column: "created_at");

      migrationBuilder.CreateIndex(
          name: "IX_monitors_enabled",
          table: "monitors",
          column: "enabled");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "monitors");
    }
  }
}
