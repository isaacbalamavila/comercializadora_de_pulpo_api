using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Purchases;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<int> GetTotalbyDateAsync(DateTime date);
        Task<PurchaseResponseDTO> GetPurchasesAsync(PurchaseRequestDTO request);
        Task<Response<Purchase>> CreatePurchaseAsync(Purchase newPurchase);

        Task<Purchase?> GetPurchaseById(Guid id);
    }
}
