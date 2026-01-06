using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IProductInventoryService
    {
        Task<Response<List<ProductInventoryDTO>>> GetProductInventoryAsync();
        Task<Response<ProductBatchInventoryResponse>> GetProductBatchInventoryAsync(
            ProductBatchesRequestDTO request
        );
        Task<Response<ProductBatchDetails>> GetProductDetailsByIdAsync(Guid productBatchId);
        Task<Response<bool>> UpdateBatchRemain(Guid supplyId, UpdateRemain request);
        Task<Response<bool>> DisposeProductBatchByIdAsync(Guid supplyId);
    }
}
