namespace comercializadora_de_pulpo_api.Models.DTOs.Proccess
{
    public class ProductionProcessDetailsDTO
    {
        public Guid Id { get; set; }
        public String ProductName { get; set; } = null!;
        public String Sku { get; set; } = null!;
        public int Status { get; set; }
        public String RawMaterial { get; set; } = null!;
        public Decimal RawMaterialNeededKg { get; set; }
        public int TimeNeededMin { get; set; }
        public int Quantity { get; set; }
        public List<ProductionSupplyDetailsDTO> SuppliesUsed { get; set; } = [];
    }

    public class ProductionSupplyDetailsDTO { 
        public String Sku { get; set; } = null!;
        public Decimal UsedWeightKg { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
