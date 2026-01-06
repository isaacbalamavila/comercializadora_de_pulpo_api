using System;
using System.Collections.Generic;

namespace comercializadora_de_pulpo_api.Models;

public partial class Sale
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int PaymentMethod { get; set; }

    public Guid EmployeeId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual User Employee { get; set; } = null!;

    public virtual PaymentMethod PaymentMethodNavigation { get; set; } = null!;

    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
