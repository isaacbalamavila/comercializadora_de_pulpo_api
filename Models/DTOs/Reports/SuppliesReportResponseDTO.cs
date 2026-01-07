namespace comercializadora_de_pulpo_api.Models.DTOs.Reports
{
    public class SuppliesReportResponseDTO : BaseReportsResponse
    {
        public List<SupplyReportItem> Supplies { get; set; } = [];
    }

    public class SupplyReportItem
    {
        public string Sku { get; set; } = null!;
        public string RawMaterial { get; set; } = null!;
        public decimal OriginalWeightKg { get; set; }
        public decimal RemainWeightKg { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
