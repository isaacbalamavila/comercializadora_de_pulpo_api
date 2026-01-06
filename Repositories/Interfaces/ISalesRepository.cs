using comercializadora_de_pulpo_api.Models.DTOs.Sale;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface ISalesRepository
    {
        Task<SalesResponseDTO> GetSalesAsync(SalesRequestDTO request);
        Task<SaleResponse?> GetSaleDetailsByIdAsync(Guid saleId);
    }
}
