using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class Client
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Rfc { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
