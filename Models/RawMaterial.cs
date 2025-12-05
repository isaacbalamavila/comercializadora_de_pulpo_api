using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class RawMaterial
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ScientificName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<SuppliesInventory> SuppliesInventories { get; set; } = new List<SuppliesInventory>();
}
