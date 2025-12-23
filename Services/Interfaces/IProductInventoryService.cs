using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IProductInventoryService
    {
        Task<Response<List<ProductInventoryDTO>>> GetProductInventoryAsync();
        Task<Response<ProductBatchInventoryResponse>> GetProductBatchInventoryAsync(ProductBatchesRequestDTO request);

    }
}
