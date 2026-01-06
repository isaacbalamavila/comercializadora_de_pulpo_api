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

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductBatch> ProductBatches { get; set; }

    public virtual DbSet<ProductBatchSupply> ProductBatchSupplies { get; set; }

    public virtual DbSet<ProductionProcess> ProductionProcesses { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<RawMaterial> RawMaterials { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleItem> SaleItems { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

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

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payment___3213E83FB2EDE84B");

            entity.ToTable("payment_methods");

            entity.HasIndex(e => e.Name, "UQ__payment___72E12F1BC68A4545").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
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

        modelBuilder.Entity<ProductBatch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__product___3213E83F738FFEDA");

            entity.ToTable("product_batches");

            entity.HasIndex(e => e.Sku, "UQ__product___DDDF4BE7D2031A51").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("expiration_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductionProcessId).HasColumnName("production_process_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Remain).HasColumnName("remain");
            entity.Property(e => e.Sku)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sku");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductBatches)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__product_b__produ__4DB4832C");

            entity.HasOne(d => d.ProductionProcess).WithMany(p => p.ProductBatches)
                .HasForeignKey(d => d.ProductionProcessId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__product_b__produ__4CC05EF3");
        });

        modelBuilder.Entity<ProductBatchSupply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__product___3213E83F4A41B983");

            entity.ToTable("product_batch_supplies");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ProductBatchId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("product_batch_id");
            entity.Property(e => e.ProductionProcessId).HasColumnName("production_process_id");
            entity.Property(e => e.SuppliesInventoryId).HasColumnName("supplies_inventory_id");
            entity.Property(e => e.UsedWeightKg)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("used_weight_kg");

            entity.HasOne(d => d.ProductBatch).WithMany(p => p.ProductBatchSupplies)
                .HasForeignKey(d => d.ProductBatchId)
                .HasConstraintName("FK__product_b__produ__695C9DA1");

            entity.HasOne(d => d.ProductionProcess).WithMany(p => p.ProductBatchSupplies)
                .HasForeignKey(d => d.ProductionProcessId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__product_b__produ__6B44E613");

            entity.HasOne(d => d.SuppliesInventory).WithMany(p => p.ProductBatchSupplies)
                .HasForeignKey(d => d.SuppliesInventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__product_b__suppl__6A50C1DA");
        });

        modelBuilder.Entity<ProductionProcess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__producti__3213E83F0814A2D9");

            entity.ToTable("production_process");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.EndDate)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductionProcesses)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__productio__produ__451F3D2B");

            entity.HasOne(d => d.Status).WithMany(p => p.ProductionProcesses)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__productio__statu__46136164");

            entity.HasOne(d => d.User).WithMany(p => p.ProductionProcesses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__productio__user___442B18F2");
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

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sales__3213E83F59FEA98A");

            entity.ToTable("sales");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.EmployeeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("employee_id");
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
            entity.Property(e => e.SaleDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("sale_date");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Client).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales__payment_m__0D99FE17");

            entity.HasOne(d => d.Employee).WithMany(p => p.Sales)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_employee");

            entity.HasOne(d => d.PaymentMethodNavigation).WithMany(p => p.Sales)
                .HasForeignKey(d => d.PaymentMethod)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sales__payment_m__0E8E2250");
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sale_ite__3213E83F4841FF44");

            entity.ToTable("sale_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sale_item__produ__1352D76D");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleItems)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sale_item__sale___125EB334");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__status__3213E83FDC28710A");

            entity.ToTable("status");

            entity.HasIndex(e => e.Label, "UQ__status__4823FDB27F58CEED").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Label)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("label");
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
            entity.Property(e => e.FirstLogin)
                .HasDefaultValue(true)
                .HasColumnName("first_login");
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
