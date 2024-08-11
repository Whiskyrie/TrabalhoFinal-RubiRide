using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportManager.Migrations;

/// <inheritdoc />
public partial class AddDriversTable : Migration {
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder) {
    migrationBuilder.CreateTable(
        name: "Drivers",
        columns: table =>
            new { Id = table.Column<string>(type: "TEXT", maxLength: 36,
                                            nullable: false),
                  LicenseNumber = table.Column<string>(
                      type: "TEXT", maxLength: 20, nullable: false),
                  LicenseExpirationDate =
                      table.Column<DateTime>(type: "TEXT", nullable: false),
                  Status = table.Column<string>(type: "TEXT", nullable: false),
                  Name = table.Column<string>(type: "TEXT", maxLength: 100,
                                              nullable: false),
                  CreatedAt =
                      table.Column<DateTime>(type: "TEXT", nullable: false),
                  UpdatedAt =
                      table.Column<DateTime>(type: "TEXT", nullable: false) },
        constraints: table => { table.PrimaryKey("PK_Drivers", x => x.Id); });
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder) {
    migrationBuilder.DropTable(name: "Drivers");
  }
}
