using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Api.Infrastructure.Persistence.Migrations;

/// <summary>
/// Aligns the EF Core snapshot with the existing schema without changing stored data.
/// </summary>
public partial class SynchronizeOrderModelMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
