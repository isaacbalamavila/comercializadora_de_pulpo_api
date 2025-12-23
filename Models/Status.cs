using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Label { get; set; } = null!;

    public virtual ICollection<ProductionProcess> ProductionProcesses { get; set; } = new List<ProductionProcess>();
}
