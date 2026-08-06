using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Order.Api.Infrastructure.Persistence.Migrations;

[DbContext(typeof(OrderDbContext))]
[Migration("202608060001_AddOrderTotalPrice")]
public partial class AddOrderTotalPrice : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "TotalPrice",
            table: "orders",
            type: "numeric",
            nullable: false,
            defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TotalPrice",
            table: "orders");
    }
}
