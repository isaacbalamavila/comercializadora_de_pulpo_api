using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Reports;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IReportService
    {
        Task<Response<SaleReportResponseDTO>> GetSaleReportAsync(ReportRequestDTO request);
        Task<Response<ClientsReportResponseDTO>> GetClientReportAsync(ReportRequestDTO request);
        Task<Response<ProductsReportResponseDTO>> GetProductReportAsync(ReportRequestDTO request);
        Task<Response<SuppliesReportResponseDTO>> GetSuppliesReportAsync(ReportRequestDTO request);
        Task<Response<PurchasesReportResponseDTO>> GetPurchaseReportAsync(ReportRequestDTO request);
    }
}
