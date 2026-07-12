using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Api.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "stock_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Available = table.Column<int>(type: "integer", nullable: false),
                Reserved = table.Column<int>(type: "integer", nullable: false),
                ReorderLevel = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_stock_items", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_stock_items_ProductId",
            table: "stock_items",
            column: "ProductId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "stock_items");
    }
}
