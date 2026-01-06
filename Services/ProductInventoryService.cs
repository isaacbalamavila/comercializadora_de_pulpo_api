using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;

namespace comercializadora_de_pulpo_api.Services
{
    public class ProductInventoryService(IProductInventoryRepository productInventoryRepository)
        : IProductInventoryService
    {
        private readonly IProductInventoryRepository _productInventoryRepository =
            productInventoryRepository;

        public async Task<Response<List<ProductInventoryDTO>>> GetProductInventoryAsync()
        {
            var inventory = await _productInventoryRepository.GetProductInventoryAsync();

            return Response<List<ProductInventoryDTO>>.Ok(inventory);
        }

        public async Task<Response<ProductBatchInventoryResponse>> GetProductBatchInventoryAsync(
            ProductBatchesRequestDTO request
        )
        {
            var inventory = await _productInventoryRepository.GetProductBatchesAsync(request);
            return Response<ProductBatchInventoryResponse>.Ok(inventory);
        }

        public async Task<Response<ProductBatchDetails>> GetProductDetailsByIdAsync(
            Guid productBatchId
        )
        {
            var productBatchSaved =
                await _productInventoryRepository.GetProductBatchDetailsByIdAsync(productBatchId);

            if (productBatchSaved == null)
                return Response<ProductBatchDetails>.Fail(
                    "No se encontró el lote del producto",
                    $"No se encontró un lote de producto con el ID:'{productBatchId}'",
                    404
                );

            return Response<ProductBatchDetails>.Ok(productBatchSaved);
        }

        public async Task<Response<bool>> UpdateBatchRemain(Guid productId, UpdateRemain request)
        {
            if (request.UpdatedRemain < 0)
                return Response<bool>.Fail(
                    "Monto inválido",
                    "El monto asignado debe ser mayor 0",
                    400
                );

            var productBatchSaved = await _productInventoryRepository.GetProductBatchByIdAsync(
                productId
            );

            if (productBatchSaved == null)
                return Response<bool>.Fail(
                    "Lote de productos no encontrado",
                    $"No se encontro un lote de productos con el ID '{productId}'",
                    404
                );

            if (productBatchSaved.Remain == 0)
                return Response<bool>.Fail(
                    "El lote se encuentra con las modificaciones restringidas",
                    "No se puede modificar un lote consumido",
                    400
                );

            if (request.UpdatedRemain > productBatchSaved.Remain)
                return Response<bool>.Fail(
                    "Monto máximo excedido",
                    "No se puede asignar un monto mayor al disponible",
                    400
                );

            if (request.UpdatedRemain == productBatchSaved.Remain)
                return Response<bool>.Ok(true, 204);

            productBatchSaved.Remain = request.UpdatedRemain;

            var updateRequest = await _productInventoryRepository.UpdateProductBatchAsync(
                productBatchSaved
            );

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true, 204)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar actualizar la cantidad disponible del lote",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<bool>> DisposeProductBatchByIdAsync(Guid productBatchId)
        {
            var productBatchSaved = await _productInventoryRepository.GetProductBatchByIdAsync(
                productBatchId
            );

            if (productBatchSaved == null)
                return Response<bool>.Fail(
                    "No se encontró el lote del producto",
                    $"No se encontró un lote de producto con el ID:'{productBatchId}'",
                    404
                );

            if (productBatchSaved.Remain == 0)
                return Response<bool>.Ok(true, 204);

            productBatchSaved.Remain = 0;

            var updateRequest = await _productInventoryRepository.UpdateProductBatchAsync(
                productBatchSaved
            );

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true, 204)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar dar de baja el lote",
                    updateRequest.Error!.ErrorDetails
                );
        }
    }
}
