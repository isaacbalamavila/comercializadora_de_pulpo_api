using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface ISuppliesInventoryService
    {
        Task<Response<SuppliesResponseDTO>> GetSuppliesAsync(SuppliesRequestDTO request);
        Task<Response<SupplyDetailsDTO>> GetSupplyByIdAsync(Guid supplyId);
        Task<Response<SupplyDTO>> UpdateWeightRemainAsync(
            Guid supplyId,
            UpdateRemain request
        );
        Task<Response<bool>> DisposeSupplyAsync(Guid supplyId);
    }
}
