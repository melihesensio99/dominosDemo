using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Migrations;

public partial class AddStockReservationsAndTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS stock_items (
                "Id" uuid PRIMARY KEY,
                "StockKey" varchar(100) NOT NULL,
                "DisplayName" varchar(200) NOT NULL DEFAULT '',
                "TrackingType" integer NOT NULL DEFAULT 0,
                "Available" integer NOT NULL,
                "Reserved" integer NOT NULL,
                "ReorderLevel" integer NOT NULL,
                "LowStockNotified" boolean NOT NULL DEFAULT false,
                "IsActive" boolean NOT NULL DEFAULT true,
                "CreatedAt" timestamptz NOT NULL,
                "UpdatedAt" timestamptz
            );

            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM information_schema.columns
                    WHERE table_name = 'stock_items' AND column_name = 'ProductId'
                ) AND NOT EXISTS (
                    SELECT 1 FROM information_schema.columns
                    WHERE table_name = 'stock_items' AND column_name = 'StockKey'
                ) THEN
                    ALTER TABLE stock_items RENAME COLUMN "ProductId" TO "StockKey";
                END IF;
            END $$;

            ALTER TABLE stock_items ADD COLUMN IF NOT EXISTS "DisplayName" varchar(200) NOT NULL DEFAULT '';
            ALTER TABLE stock_items ADD COLUMN IF NOT EXISTS "TrackingType" integer NOT NULL DEFAULT 0;
            ALTER TABLE stock_items ADD COLUMN IF NOT EXISTS "LowStockNotified" boolean NOT NULL DEFAULT false;
            ALTER TABLE stock_items ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT true;

            DROP INDEX IF EXISTS "IX_stock_items_ProductId";
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_stock_items_StockKey" ON stock_items ("StockKey");

            CREATE TABLE IF NOT EXISTS stock_reservations (
                "OrderId" varchar(32) PRIMARY KEY,
                "Status" integer NOT NULL,
                "ItemsJson" text NOT NULL,
                "CreatedAt" timestamptz NOT NULL,
                "UpdatedAt" timestamptz
            );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DROP TABLE IF EXISTS stock_reservations;
            ALTER TABLE stock_items DROP COLUMN IF EXISTS "DisplayName";
            ALTER TABLE stock_items DROP COLUMN IF EXISTS "TrackingType";
            ALTER TABLE stock_items DROP COLUMN IF EXISTS "LowStockNotified";
            ALTER TABLE stock_items DROP COLUMN IF EXISTS "IsActive";
            """);
    }
}
