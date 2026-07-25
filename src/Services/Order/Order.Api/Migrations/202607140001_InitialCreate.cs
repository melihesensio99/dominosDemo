using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Order.Api.Migrations;

[Migration("202607140001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "orders",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CustomerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                shipping_street = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                shipping_district = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                shipping_city = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                shipping_postal_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                shipping_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                billing_street = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                billing_district = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                billing_city = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                billing_postal_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                billing_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                payment_method = table.Column<int>(type: "integer", nullable: false),
                payment_status = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_orders", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "order_items",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                OrderId = table.Column<string>(type: "character varying(32)", nullable: false),
                ProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_order_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_order_items_orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ProcessingAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                RetryCount = table.Column<int>(type: "integer", nullable: false),
                Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Payload = table.Column<string>(type: "text", nullable: false),
                ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                FailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_outbox_messages", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_order_items_OrderId",
            table: "order_items",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_outbox_messages_OccurredAt",
            table: "outbox_messages",
            column: "OccurredAt");

        migrationBuilder.CreateIndex(
            name: "IX_outbox_messages_LockedAt",
            table: "outbox_messages",
            column: "LockedAt");

        migrationBuilder.CreateIndex(
            name: "IX_outbox_messages_FailedAt",
            table: "outbox_messages",
            column: "FailedAt");

        migrationBuilder.CreateIndex(
            name: "IX_outbox_messages_ProcessedAt",
            table: "outbox_messages",
            column: "ProcessedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "outbox_messages");

        migrationBuilder.DropTable(
            name: "order_items");

        migrationBuilder.DropTable(
            name: "orders");
    }
}
