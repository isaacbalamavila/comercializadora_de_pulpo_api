using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Sale;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface ISalesService
    {
        Task<Response<SalesResponseDTO>> GetSalesAsync(SalesRequestDTO request);
        Task<Response<SaleResponse>> GetSaleDetailsByIdAsync(Guid id);
        Task<Response<SaleResponse>> SaveSale(SaleRequest request);
    }
}
