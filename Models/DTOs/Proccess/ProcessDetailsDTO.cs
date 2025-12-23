namespace comercializadora_de_pulpo_api.Models.DTOs.Proccess
{
    public class ProcessDetailsDTO
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ProductProcess Product { get; set; } = null!;

        public StatusProcess Status { get; set; } = null!;

        public UserProcess User { get; set; } = null!;
    }

    public class ProductProcess {
        public string SKU { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string RawMaterial {  get; set; } = null!;
        public decimal RawMaterialNeededKg { get; set; }
        public int Content {  get; set; }
        public string Unit {  get; set; } = null!;
    }

    public class UserProcess {
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    public class StatusProcess {
        public int Id { get; set; }
        public string Label { get; set; } = null!;
    }


}
