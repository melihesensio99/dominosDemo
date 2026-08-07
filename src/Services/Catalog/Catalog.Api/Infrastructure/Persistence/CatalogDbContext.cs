using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure.Persistence;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<ProductOptionGroup> ProductOptionGroups => Set<ProductOptionGroup>();

    public DbSet<ProductOption> ProductOptions => Set<ProductOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Slug).IsRequired().HasMaxLength(220);
            entity.Property(x => x.IsActive).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
            entity.HasIndex(x => x.Slug).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.ImageUrl).HasMaxLength(1000);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Property(x => x.Stock).IsRequired();
            entity.Property(x => x.InventoryTrackingType).IsRequired();
            entity.Property(x => x.InventoryKey).HasMaxLength(100);
            entity.Property(x => x.IsActive).IsRequired();
            entity.Property(x => x.CategoryId).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.HasIndex(x => x.CategoryId);
            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductOptionGroup>(entity =>
        {
            entity.ToTable("product_option_groups");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.SelectionType).IsRequired().HasMaxLength(20);
            entity.Property(x => x.IsRequired).IsRequired();
            entity.Property(x => x.DisplayOrder).IsRequired();
            entity.HasIndex(x => new { x.ProductId, x.DisplayOrder });
            entity.HasOne(x => x.Product)
                .WithMany(x => x.OptionGroups)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductOption>(entity =>
        {
            entity.ToTable("product_options");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.PriceAdjustment).HasPrecision(18, 2);
            entity.Property(x => x.InventoryKey).HasMaxLength(100);
            entity.Property(x => x.IsDefault).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
            entity.Property(x => x.DisplayOrder).IsRequired();
            entity.HasIndex(x => new { x.ProductOptionGroupId, x.DisplayOrder });
            entity.HasOne(x => x.ProductOptionGroup)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.ProductOptionGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
