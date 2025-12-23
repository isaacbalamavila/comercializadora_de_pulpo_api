using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Products
{
    public class ProductBatchInventoryResponse: PaginationResponse
    {
        public List<ProductBatchInventory> Productbatches { get; set; } = [];
    }
    public class ProductBatchInventory
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Content { get; set; }
        public string Unit { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityRemain { get; set; }
    }
}
