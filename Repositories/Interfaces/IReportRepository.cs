using comercializadora_de_pulpo_api.Models.DTOs.Reports;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<SaleReportResponseDTO> GetSalesReportAsync(ReportRequestDTO request);
        Task<ClientsReportResponseDTO> GetClientReportAsync(ReportRequestDTO request);
        Task<ProductsReportResponseDTO> GetProductReportAsync(ReportRequestDTO request);
        Task<SuppliesReportResponseDTO> GetSuppliesReportAsync(ReportRequestDTO request);
        Task<PurchasesReportResponseDTO> GetPurchasesReportAsync(ReportRequestDTO request);
    }
}
