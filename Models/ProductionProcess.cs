using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class ProductionProcess
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int StatusId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductBatchSupply> ProductBatchSupplies { get; set; } = new List<ProductBatchSupply>();

    public virtual ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();

    public virtual Status Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
