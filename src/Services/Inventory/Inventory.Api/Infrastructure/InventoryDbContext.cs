using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.ToTable("stock_items");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProductId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Available).IsRequired();
            entity.Property(x => x.Reserved).IsRequired();
            entity.Property(x => x.ReorderLevel).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt);
            entity.HasIndex(x => x.ProductId).IsUnique();
        });
    }
}
