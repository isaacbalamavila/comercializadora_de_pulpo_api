using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface ISuppliesInventoryRepository
    {
        Task<int> GetTotalbyDateAsync(DateTime date);
        Task<List<SuppliesInventory>> GetSuppliesForProductionAsync(int rawMaterialId);
        Task<SuppliesResponseDTO> GetSuppliesAsync(SuppliesRequestDTO request);
        Task<SuppliesInventory?> GetSupplyByIdAsync(Guid id);
        Task<Response<SuppliesInventory>> CreateSupplieAsync(SuppliesInventory newSupplier);
        Task<Response<SuppliesInventory>> UpdateSupplyAsync(SuppliesInventory UpdatedSupply);
    }
}
