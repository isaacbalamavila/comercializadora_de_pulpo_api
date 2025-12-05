namespace comercializadora_de_pulpo_api.Models.DTOs.Purchases
{
    public class PurchaseDetailsDTO
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public string Supplier { get; set; } = null!;
        public string SupplierEmail { get; set; } = null!;
        public string SupplierPhone { get; set; }= null!;
        public string RawMaterial { get; set; } = null!;
        public string RawMaterialDescription { get; set; } = null!;
        public decimal TotalKg { get; set; }
        public decimal PriceKg { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
