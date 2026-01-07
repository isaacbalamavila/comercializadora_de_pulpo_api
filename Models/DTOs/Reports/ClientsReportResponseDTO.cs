namespace comercializadora_de_pulpo_api.Models.DTOs.Reports
{
    public class ClientsReportResponseDTO : BaseReportsResponse
    {
        public List<ClientReportItem> Clients { get; set; } = [];
    }

    public class ClientReportItem
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Rfc { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
