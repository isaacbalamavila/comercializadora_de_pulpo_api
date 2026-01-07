namespace comercializadora_de_pulpo_api.Models.DTOs.Reports
{
    public class SaleReportResponseDTO : BaseReportsResponse
    {
        public List<SaleReportItem> Sales { get; set; } = [];
    }

    public class SaleReportItem
    {
        public string Client { get; set; } = null!;
        public string Employee { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }
}
