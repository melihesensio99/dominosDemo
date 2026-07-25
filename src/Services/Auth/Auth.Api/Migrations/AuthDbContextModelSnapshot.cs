using System;
using Auth.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Auth.Api.Migrations;

[DbContext(typeof(AuthDbContext))]
partial class AuthDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.9");

        modelBuilder.Entity("Auth.Api.Domain.User", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnType("uuid");

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnType("character varying(256)");

            b.Property<string>("PasswordHash")
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnType("character varying(512)");

            b.Property<string>("Role")
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnType("character varying(64)");

            b.HasKey("Id");

            b.HasIndex("Email")
                .IsUnique();

            b.ToTable("auth_users", (string)null);
        });

        modelBuilder.Entity("Auth.Api.Domain.UserAddress", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnName("id")
                .HasColumnType("uuid");

            b.Property<string>("City")
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("city")
                .HasColumnType("character varying(150)");

            b.Property<string>("Country")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("country")
                .HasColumnType("character varying(100)");

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("District")
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("district")
                .HasColumnType("character varying(150)");

            b.Property<string>("PostalCode")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("postal_code")
                .HasColumnType("character varying(50)");

            b.Property<string>("Street")
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnName("street")
                .HasColumnType("character varying(250)");

            b.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("title")
                .HasColumnType("character varying(100)");

            b.Property<Guid>("UserId")
                .HasColumnName("user_id")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("UserId")
                .HasDatabaseName("ix_user_addresses_user_id");

            b.ToTable("user_addresses", (string)null);
        });
#pragma warning restore 612, 618
    }
}
