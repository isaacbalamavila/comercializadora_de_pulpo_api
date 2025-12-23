namespace comercializadora_de_pulpo_api.Models.DTOs.Products
{
    public class ProductInventoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string RawMaterial { get; set; } = null!;
        public decimal Price { get; set; }
        public int Content { get; set; }
        public string Unit { get; set; } = null!;
        public int StockMin { get; set; }
        public decimal Quantity { get; set; }

    }
}
