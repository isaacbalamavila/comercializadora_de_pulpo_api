namespace comercializadora_de_pulpo_api.Models.DTOs.Products
{
    public class ProductBatchDetails
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public Guid ProcessId { get; set; }
        public ProductInfo Product { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal quantity { get; set; }
        public decimal quantityRemain { get; set; }

    }

    public class ProductInfo {
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Content { get; set; }
        public string Unit { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
