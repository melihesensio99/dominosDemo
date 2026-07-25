using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Api.Migrations;

public partial class _202607250001_AddUserAddresses : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "user_addresses",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                street = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                district = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                city = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                postal_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_addresses", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_user_addresses_user_id",
            table: "user_addresses",
            column: "user_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "user_addresses");
    }
}
