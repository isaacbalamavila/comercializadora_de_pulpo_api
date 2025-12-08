namespace comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory
{
    public class SupplyDTO
    {
        public Guid Id { get; set; }

        public string Sku { get; set; } = null!;

        public string RawMaterial { get; set; } = null!;

        public decimal WeightRemainKg { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
