namespace comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory
{
    public class SupplyDetailsDTO
    {
        public Guid Id { get; set; }

        public string Sku { get; set; } = null!;

        public decimal WeightKg { get; set; }

        public decimal WeightRemainKg { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string PurchaseSku { get; set; } = null!;

        public string RawMaterial { get; set; } = null!;
    }
}
