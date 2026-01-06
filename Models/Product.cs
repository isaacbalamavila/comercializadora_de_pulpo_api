using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Img { get; set; } = null!;

    public int RawMaterialId { get; set; }

    public int Content { get; set; }

    public int UnitId { get; set; }

    public decimal Price { get; set; }

    public int StockMin { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal RawMaterialNeededKg { get; set; }

    public int TimeNeededMin { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();

    public virtual ICollection<ProductionProcess> ProductionProcesses { get; set; } = new List<ProductionProcess>();

    public virtual RawMaterial RawMaterial { get; set; } = null!;

    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    public virtual Unit Unit { get; set; } = null!;
}
