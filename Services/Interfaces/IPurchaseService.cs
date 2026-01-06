using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Purchases;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<Response<PurchaseResponseDTO>> GetPurchasesAsync(PurchaseRequestDTO request);
        Task<Response<PurchaseDTO>> SavePurchase(CreatePurchaseDTO request);
        Task<Response<PurchaseDetailsDTO>> GetPurchaseByIdAsync(Guid id);
    }
}
