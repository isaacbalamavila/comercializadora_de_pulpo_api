using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Reports;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;

namespace comercializadora_de_pulpo_api.Services
{
    public class ReportService(IReportRepository reportRepository) : IReportService
    {
        private readonly IReportRepository _reportRepository = reportRepository;

        public async Task<Response<SaleReportResponseDTO>> GetSaleReportAsync(
            ReportRequestDTO request
        )
        {
            var report = await _reportRepository.GetSalesReportAsync(request);
            return Response<SaleReportResponseDTO>.Ok(report);
        }

        public async Task<Response<ClientsReportResponseDTO>> GetClientReportAsync(
            ReportRequestDTO request
        )
        {
            var report = await _reportRepository.GetClientReportAsync(request);

            return Response<ClientsReportResponseDTO>.Ok(report);
        }

        public async Task<Response<ProductsReportResponseDTO>> GetProductReportAsync(
            ReportRequestDTO request
        )
        {
            var report = await _reportRepository.GetProductReportAsync(request);

            return Response<ProductsReportResponseDTO>.Ok(report);
        }

        public async Task<Response<SuppliesReportResponseDTO>> GetSuppliesReportAsync(
            ReportRequestDTO request
        )
        {
            var report = await _reportRepository.GetSuppliesReportAsync(request);

            return Response<SuppliesReportResponseDTO>.Ok(report);
        }

        public async Task<Response<PurchasesReportResponseDTO>> GetPurchaseReportAsync(
            ReportRequestDTO request
        )
        {
            var report = await _reportRepository.GetPurchasesReportAsync(request);

            return Response<PurchasesReportResponseDTO>.Ok(report);
        }
    }
}
