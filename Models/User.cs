using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public bool FirstLogin { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RoleId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ProductionProcess> ProductionProcesses { get; set; } = new List<ProductionProcess>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual Role Role { get; set; } = null!;
}
