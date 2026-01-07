namespace comercializadora_de_pulpo_api.Models.DTOs.Reports
{
    public class PurchasesReportResponseDTO : BaseReportsResponse
    {
        public List<PurchaseReportItem> Purchases { get; set; } = [];
    }

    public class PurchaseReportItem
    {
        public string Sku { get; set; } = null!;
        public string Supplier { get; set; } = null!;
        public string RawMaterial { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal TotalKg { get; set; }
        public decimal PriceKG { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
