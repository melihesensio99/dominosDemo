using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<StockItem> StockItems => Set<StockItem>();

    public DbSet<StockReservation> StockReservations => Set<StockReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.ToTable("stock_items");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StockKey).IsRequired().HasMaxLength(100);
            entity.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.TrackingType).IsRequired();
            entity.Property(x => x.Available).IsRequired();
            entity.Property(x => x.Reserved).IsRequired();
            entity.Property(x => x.ReorderLevel).IsRequired();
            entity.Property(x => x.LowStockNotified).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt);
            entity.HasIndex(x => x.StockKey).IsUnique();
        });

        modelBuilder.Entity<StockReservation>(entity =>
        {
            entity.ToTable("stock_reservations");
            entity.HasKey(x => x.OrderId);
            entity.Property(x => x.OrderId).HasMaxLength(32).ValueGeneratedNever();
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.ItemsJson).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt);
        });
    }
}
