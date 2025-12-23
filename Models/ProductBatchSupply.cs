using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class ProductBatchSupply
{
    public Guid Id { get; set; }

    public Guid ProductionProcessId { get; set; }

    public Guid? ProductBatchId { get; set; }

    public Guid SuppliesInventoryId { get; set; }

    public decimal UsedWeightKg { get; set; }

    public virtual ProductBatch? ProductBatch { get; set; }

    public virtual ProductionProcess ProductionProcess { get; set; } = null!;

    public virtual SuppliesInventory SuppliesInventory { get; set; } = null!;
}
