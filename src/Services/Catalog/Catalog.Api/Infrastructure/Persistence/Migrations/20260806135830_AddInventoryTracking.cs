using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Api.Infrastructure.Persistence.Migrations;

public partial class AddInventoryTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS categories (
                "Id" uuid PRIMARY KEY,
                "Name" varchar(200) NOT NULL,
                "Slug" varchar(220) NOT NULL,
                "IsActive" boolean NOT NULL,
                "CreatedAt" timestamptz NOT NULL
            );

            CREATE TABLE IF NOT EXISTS products (
                "Id" uuid PRIMARY KEY,
                "Name" varchar(250) NOT NULL,
                "Description" varchar(2000) NOT NULL,
                "ImageUrl" varchar(1000),
                "Price" numeric(18,2) NOT NULL,
                "Stock" integer NOT NULL,
                "InventoryTrackingType" integer NOT NULL DEFAULT 0,
                "InventoryKey" varchar(100),
                "IsActive" boolean NOT NULL,
                "CategoryId" uuid NOT NULL REFERENCES categories("Id") ON DELETE RESTRICT,
                "CreatedAt" timestamptz NOT NULL,
                "UpdatedAt" timestamptz
            );

            ALTER TABLE products ADD COLUMN IF NOT EXISTS "ImageUrl" varchar(1000);
            ALTER TABLE products ADD COLUMN IF NOT EXISTS "InventoryTrackingType" integer NOT NULL DEFAULT 0;
            ALTER TABLE products ADD COLUMN IF NOT EXISTS "InventoryKey" varchar(100);

            CREATE TABLE IF NOT EXISTS product_option_groups (
                "Id" uuid PRIMARY KEY,
                "ProductId" uuid NOT NULL REFERENCES products("Id") ON DELETE CASCADE,
                "Name" varchar(150) NOT NULL,
                "SelectionType" varchar(20) NOT NULL,
                "IsRequired" boolean NOT NULL,
                "DisplayOrder" integer NOT NULL
            );

            CREATE TABLE IF NOT EXISTS product_options (
                "Id" uuid PRIMARY KEY,
                "ProductOptionGroupId" uuid NOT NULL REFERENCES product_option_groups("Id") ON DELETE CASCADE,
                "Name" varchar(150) NOT NULL,
                "PriceAdjustment" numeric(18,2) NOT NULL,
                "InventoryKey" varchar(100),
                "IsDefault" boolean NOT NULL,
                "IsActive" boolean NOT NULL,
                "DisplayOrder" integer NOT NULL
            );

            ALTER TABLE product_options ADD COLUMN IF NOT EXISTS "InventoryKey" varchar(100);
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_categories_Name" ON categories ("Name");
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_categories_Slug" ON categories ("Slug");
            CREATE INDEX IF NOT EXISTS "IX_products_CategoryId" ON products ("CategoryId");
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_products_Name" ON products ("Name");
            CREATE INDEX IF NOT EXISTS "IX_product_option_groups_ProductId_DisplayOrder" ON product_option_groups ("ProductId", "DisplayOrder");
            CREATE INDEX IF NOT EXISTS "IX_product_options_ProductOptionGroupId_DisplayOrder" ON product_options ("ProductOptionGroupId", "DisplayOrder");
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE product_options DROP COLUMN IF EXISTS "InventoryKey";
            ALTER TABLE products DROP COLUMN IF EXISTS "InventoryKey";
            ALTER TABLE products DROP COLUMN IF EXISTS "InventoryTrackingType";
            """);
    }
}
