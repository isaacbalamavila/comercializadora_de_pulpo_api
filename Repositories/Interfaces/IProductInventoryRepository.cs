using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface IProductInventoryRepository
    {
        Task<int> GetTotalbyDateAsync(DateTime date);
        Task<Response<ProductBatch>> CreateProductBatchAsync(ProductBatch newProductBatch);
        Task<List<ProductInventoryDTO>> GetProductInventoryAsync();
        Task<ProductBatchInventoryResponse> GetProductBatchesAsync(
            ProductBatchesRequestDTO request
        );
    }
}
