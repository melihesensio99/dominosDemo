using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Order.Api.Infrastructure.Persistence.Migrations;

[Migration("202607250001_AddOrderNoteAndSelectedOptions")]
public partial class AddOrderNoteAndSelectedOptions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Note",
            table: "orders",
            type: "character varying(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "SelectedOptionIdsJson",
            table: "order_items",
            type: "character varying(4000)",
            maxLength: 4000,
            nullable: false,
            defaultValue: "[]");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Note", table: "orders");
        migrationBuilder.DropColumn(name: "SelectedOptionIdsJson", table: "order_items");
    }
}
