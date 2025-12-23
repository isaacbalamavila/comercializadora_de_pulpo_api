using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class SuppliesInventory
{
    public Guid Id { get; set; }

    public Guid PurchaseId { get; set; }

    public string Sku { get; set; } = null!;

    public int RawMaterialId { get; set; }

    public decimal WeightKg { get; set; }

    public decimal WeightRemainKg { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public virtual ICollection<ProductBatchSupply> ProductBatchSupplies { get; set; } = new List<ProductBatchSupply>();

    public virtual Purchase Purchase { get; set; } = null!;

    public virtual RawMaterial RawMaterial { get; set; } = null!;
}
