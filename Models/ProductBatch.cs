using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class ProductBatch
{
    public Guid Id { get; set; }

    public Guid ProductionProcessId { get; set; }

    public Guid ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpirationDate { get; set; }

    public int Remain { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductBatchSupply> ProductBatchSupplies { get; set; } = new List<ProductBatchSupply>();

    public virtual ProductionProcess ProductionProcess { get; set; } = null!;
}
