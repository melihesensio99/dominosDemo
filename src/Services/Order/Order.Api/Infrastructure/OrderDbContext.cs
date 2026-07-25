using Microsoft.EntityFrameworkCore;
using OrderEntity = Order.Api.Domain.Order;

namespace Order.Api.Infrastructure;

public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.Ignore(x => x.DomainEvents);
            entity.ToTable("orders");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever().IsRequired().HasMaxLength(32);
            entity.Property(x => x.CustomerId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt);
            entity.Property(x => x.Note).IsRequired().HasMaxLength(500);

            entity.OwnsOne(x => x.ShippingAddress, address =>
            {
                address.Property(x => x.Street).HasColumnName("shipping_street").IsRequired().HasMaxLength(250);
                address.Property(x => x.District).HasColumnName("shipping_district").IsRequired().HasMaxLength(150);
                address.Property(x => x.City).HasColumnName("shipping_city").IsRequired().HasMaxLength(150);
                address.Property(x => x.PostalCode).HasColumnName("shipping_postal_code").IsRequired().HasMaxLength(50);
                address.Property(x => x.Country).HasColumnName("shipping_country").IsRequired().HasMaxLength(100);
            });

            entity.OwnsOne(x => x.BillingAddress, address =>
            {
                address.Property(x => x.Street).HasColumnName("billing_street").IsRequired().HasMaxLength(250);
                address.Property(x => x.District).HasColumnName("billing_district").IsRequired().HasMaxLength(150);
                address.Property(x => x.City).HasColumnName("billing_city").IsRequired().HasMaxLength(150);
                address.Property(x => x.PostalCode).HasColumnName("billing_postal_code").IsRequired().HasMaxLength(50);
                address.Property(x => x.Country).HasColumnName("billing_country").IsRequired().HasMaxLength(100);
            });

            entity.OwnsOne(x => x.Payment, payment =>
            {
                payment.Property(x => x.Method).HasColumnName("payment_method").IsRequired();
                payment.Property(x => x.Status).HasColumnName("payment_status").IsRequired();
            });

            entity.OwnsMany(x => x.Items, items =>
            {
                items.ToTable("order_items");
                items.WithOwner().HasForeignKey("OrderId");
                items.Property<int>("Id").ValueGeneratedOnAdd();
                items.HasKey("Id");
                items.Property(x => x.ProductId).IsRequired().HasMaxLength(100);
                items.Property(x => x.Quantity).IsRequired();
                items.Property(x => x.SelectedOptionIdsJson).IsRequired().HasMaxLength(4000);
                items.HasIndex("OrderId");
            });
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OccurredAt).IsRequired();
            entity.Property(x => x.ProcessingAt);
            entity.Property(x => x.LockedAt);
            entity.Property(x => x.RetryCount).IsRequired();
            entity.Property(x => x.Type).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Payload).IsRequired();
            entity.Property(x => x.ProcessedAt);
            entity.Property(x => x.FailedAt);
            entity.Property(x => x.Error).HasMaxLength(2000);
            entity.HasIndex(x => x.ProcessedAt);
            entity.HasIndex(x => x.FailedAt);
            entity.HasIndex(x => x.LockedAt);
            entity.HasIndex(x => x.OccurredAt);
        });
    }
}
