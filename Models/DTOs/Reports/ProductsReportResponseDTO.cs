namespace comercializadora_de_pulpo_api.Models.DTOs.Reports
{
    public class ProductsReportResponseDTO:BaseReportsResponse
    {
        public List<ProductReportItem> Products { get; set; } = [];
    }

    public class ProductReportItem {
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Content {  get; set; }
        public string Unit { get; set; } = null!;
        public int Quantity { get; set; }
        public int Remain { get; set; }
    }
}
