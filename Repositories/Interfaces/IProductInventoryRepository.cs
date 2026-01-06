using System.Threading.Tasks;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;

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
        Task<ProductBatchDetails?> GetProductBatchDetailsByIdAsync(Guid ProductBatchId);
        Task<ProductBatch?> GetProductBatchByIdAsync(Guid productBatchId);
        Task<Response<ProductBatch>> UpdateProductBatchAsync(ProductBatch productBatchUpdated);
    }
}
