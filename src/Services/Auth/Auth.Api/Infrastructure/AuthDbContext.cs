using Auth.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Infrastructure;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("auth_users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Email).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(256);
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512);
            entity.Property(x => x.Role).IsRequired();
            entity.Property(x => x.Role).HasMaxLength(64);
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.ToTable("user_addresses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(x => x.Title).HasColumnName("title").IsRequired().HasMaxLength(100);
            entity.Property(x => x.Street).HasColumnName("street").IsRequired().HasMaxLength(250);
            entity.Property(x => x.District).HasColumnName("district").IsRequired().HasMaxLength(150);
            entity.Property(x => x.City).HasColumnName("city").IsRequired().HasMaxLength(150);
            entity.Property(x => x.PostalCode).HasColumnName("postal_code").IsRequired().HasMaxLength(50);
            entity.Property(x => x.Country).HasColumnName("country").IsRequired().HasMaxLength(100);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.HasIndex(x => x.UserId);
        });
    }
}
