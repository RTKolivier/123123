using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace diplomdemo.Models;

public partial class OrbitContext : DbContext
{
    public OrbitContext()
    {
    }

    public OrbitContext(DbContextOptions<OrbitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fandom> Fandoms { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=176.123.161.45;Port=5432;Database=orbit;Password=orbit;Username=orbit");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fandom>(entity =>
        {
            entity.HasKey(e => e.FandomsId).HasName("fandoms_pkey");

            entity.ToTable("fandoms", "orbit");

            entity.Property(e => e.FandomsId)
                .HasDefaultValueSql("nextval('fandoms_fandoms_id_seq'::regclass)")
                .HasColumnName("fandoms_id");
            entity.Property(e => e.FandomsName).HasColumnName("fandoms_name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("product_pkey");

            entity.ToTable("product", "orbit");

            entity.Property(e => e.ProductId)
                .HasDefaultValueSql("nextval('product_product_id_seq'::regclass)")
                .HasColumnName("product_id");
            entity.Property(e => e.ProductCost).HasColumnName("product_cost");
            entity.Property(e => e.ProductFandom).HasColumnName("product_fandom");
            entity.Property(e => e.ProductName).HasColumnName("product_name");
            entity.Property(e => e.ProductPhoto).HasColumnName("product_photo");
            entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity");
            entity.Property(e => e.ProductSale).HasColumnName("product_sale");
            entity.Property(e => e.ProductSize).HasColumnName("product_size");
            entity.Property(e => e.ProductType).HasColumnName("product_type");

            entity.HasOne(d => d.ProductFandomNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductFandom)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_product_fandom_fkey");

            entity.HasOne(d => d.ProductSizeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductSize)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_product_size_fkey");

            entity.HasOne(d => d.ProductTypeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductType)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_product_type_fkey");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.ProductSizeId).HasName("product_size_pkey");

            entity.ToTable("product_size", "orbit");

            entity.Property(e => e.ProductSizeId)
                .HasDefaultValueSql("nextval('product_size_product_size_id_seq'::regclass)")
                .HasColumnName("product_size_id");
            entity.Property(e => e.ProductSizeName).HasColumnName("product_size_name");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("product_type_pkey");

            entity.ToTable("product_type", "orbit");

            entity.Property(e => e.ProductTypeId)
                .HasDefaultValueSql("nextval('product_type_product_type_id_seq'::regclass)")
                .HasColumnName("product_type_id");
            entity.Property(e => e.ProductTypeName).HasColumnName("product_type_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");

            entity.ToTable("role", "orbit");

            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("nextval('role_role_id_seq'::regclass)")
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.ToTable("user", "orbit");

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("nextval('user_user_id_seq'::regclass)")
                .HasColumnName("user_id");
            entity.Property(e => e.UserLogin).HasColumnName("user_login");
            entity.Property(e => e.UserName).HasColumnName("user_name");
            entity.Property(e => e.UserPassword).HasColumnName("user_password");
            entity.Property(e => e.UserPhone).HasColumnName("user_phone");
            entity.Property(e => e.UserRole).HasColumnName("user_role");

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRole)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_user_role_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
