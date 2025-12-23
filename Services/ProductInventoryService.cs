using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;

namespace comercializadora_de_pulpo_api.Services
{
    public class ProductInventoryService(IProductInventoryRepository productInventoryRepository) : IProductInventoryService
    {
        private readonly IProductInventoryRepository _productInventoryRepository = productInventoryRepository;

        public async Task<Response<List<ProductInventoryDTO>>> GetProductInventoryAsync() { 
            
          var inventory = await _productInventoryRepository.GetProductInventoryAsync();
            
            return Response<List<ProductInventoryDTO>>.Ok(inventory);
        }

        public async Task<Response<ProductBatchInventoryResponse>> GetProductBatchInventoryAsync(ProductBatchesRequestDTO request) {
            var inventory = await _productInventoryRepository.GetProductBatchesAsync(request);
            return Response<ProductBatchInventoryResponse>.Ok(inventory);
        }
    }
}
