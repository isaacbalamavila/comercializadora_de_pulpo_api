using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IProductService
    {
        Task<Response<List<ProductDTO>>> GetProductsAsync(bool onlyActives);
        Task<Response<ProductDetailsDTO>> GetProductByIdAsync(Guid productId);
        Task<Response<ProductDTO>> CreateProductAsync(CreateProductDTO request);
        Task<Response<ProductDetailsDTO>> UpdateProductAsync(
            Guid productId,
            UpdateProductDTO request
        );
        Task<Response<bool>> DeleteProductAsync(Guid productId);
        Task<Response<bool>> RestoreProductAsync(Guid productId);
    }
}
