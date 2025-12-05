using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Models;

public partial class ComercializadoraDePulpoContext : DbContext
{
    public ComercializadoraDePulpoContext()
    {
    }

    public ComercializadoraDePulpoContext(DbContextOptions<ComercializadoraDePulpoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<RawMaterial> RawMaterials { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SuppliesInventory> SuppliesInventories { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__clients__3213E83F7E9F8A75");

            entity.ToTable("clients");

            entity.HasIndex(e => e.Email, "UQ__clients__AB6E61649ABAEC02").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__clients__B43B145F856B4BEB").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Rfc)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("rfc");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83F91173360");

            entity.ToTable("products");

            entity.HasIndex(e => e.Name, "UQ__products__72E12F1B5A43BF72").IsUnique();

            entity.HasIndex(e => e.Sku, "UQ__products__DDDF4BE73BFF0B98").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Img)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("img");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("price");
            entity.Property(e => e.RawMaterialId).HasColumnName("raw_material_id");
            entity.Property(e => e.RawMaterialNeededKg)
                .HasColumnType("decimal(5, 3)")
                .HasColumnName("raw_material_needed_kg");
            entity.Property(e => e.Sku)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sku");
            entity.Property(e => e.StockMin)
                .HasDefaultValue(1)
                .HasColumnName("stock_min");
            entity.Property(e => e.TimeNeededMin).HasColumnName("time_needed_min");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.RawMaterial).WithMany(p => p.Products)
                .HasForeignKey(d => d.RawMaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__products__raw_ma__2AA05119");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__products__unit_i__2B947552");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__purchase__3213E83F04BDDCC7");

            entity.ToTable("purchases");

            entity.HasIndex(e => e.Sku, "UQ__purchase__DDDF4BE74DC7C8A7").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PriceKg)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("price_kg");
            entity.Property(e => e.RawMaterialId).HasColumnName("raw_material_id");
            entity.Property(e => e.Sku)
                .HasMaxLength(14)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sku");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TotalKg)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("total_kg");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.RawMaterial).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.RawMaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchases__raw_m__07220AB2");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchases__suppl__062DE679");

            entity.HasOne(d => d.User).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__purchases__user___08162EEB");
        });

        modelBuilder.Entity<RawMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__raw_mate__3213E83FADA2CBDE");

            entity.ToTable("raw_materials");

            entity.HasIndex(e => e.Name, "UQ__raw_mate__72E12F1B31E443BB").IsUnique();

            entity.HasIndex(e => e.ScientificName, "UQ__raw_mate__A12616E1CD8070E2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("PP")
                .IsFixedLength()
                .HasColumnName("abbreviation");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("Sin descripción")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("scientific_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F20D23A53");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "UQ__roles__72E12F1BD1B8BD46").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__supplier__3213E83FD16A12F0");

            entity.ToTable("suppliers");

            entity.HasIndex(e => e.Name, "UQ__supplier__72E12F1B0F757CFD").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__supplier__AB6E616445E93623").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__supplier__B43B145F8719552A").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AltEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("alt_email");
            entity.Property(e => e.AltPhone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .IsFixedLength()
                .HasColumnName("alt_phone");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Rfc)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("rfc");
        });

        modelBuilder.Entity<SuppliesInventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__supplies__3213E83F382F94FA");

            entity.ToTable("supplies_inventory");

            entity.HasIndex(e => e.Sku, "UQ__supplies__DDDF4BE7058CBE26").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ExpirationDate)
                .HasDefaultValueSql("(dateadd(month,(10),getdate()))")
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("purchase_date");
            entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");
            entity.Property(e => e.RawMaterialId).HasColumnName("raw_material_id");
            entity.Property(e => e.Sku)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sku");
            entity.Property(e => e.WeightKg)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("weight_kg");
            entity.Property(e => e.WeightRemainKg)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("weight_remain_kg");

            entity.HasOne(d => d.Purchase).WithMany(p => p.SuppliesInventories)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__supplies___purch__0EC32C7A");

            entity.HasOne(d => d.RawMaterial).WithMany(p => p.SuppliesInventories)
                .HasForeignKey(d => d.RawMaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__supplies___raw_m__0FB750B3");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__units__3213E83F31FF51A2");

            entity.ToTable("units");

            entity.HasIndex(e => e.Abbreviation, "UQ__units__496A048444064E96").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("abbreviation");
            entity.Property(e => e.Label)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("label");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FDF475EE0");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164E3D7D041").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstLogin).HasColumnName("first_login");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__users__role_id__7755B73D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
