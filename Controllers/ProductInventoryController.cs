using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/product-inventory")]
    [ApiController]
    public class ProductInventoryController (IProductInventoryService productInventoryService): ControllerBase
    {
        private readonly IProductInventoryService _productInventoryService = productInventoryService;
        private IActionResult HandleResponse<T>(Response<T> response, object? successData = null)
        {
            if (response.IsSuccess)
                return response.StatusCode switch
                {
                    204 => NoContent(),
                    201 => StatusCode(201, successData ?? response.Data),
                    _ => Ok(successData ?? response.Data),
                };

            return response.StatusCode switch
            {
                400 => BadRequest(response.Error),
                404 => NotFound(response.Error),
                _ => StatusCode(500, response.Error),
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory() {
            return HandleResponse(await _productInventoryService.GetProductInventoryAsync());        
        }

        [HttpGet("batches")]
        public async Task<IActionResult> GetBatchesInventory([FromQuery]ProductBatchesRequestDTO request) {
            return HandleResponse(await _productInventoryService.GetProductBatchInventoryAsync(request));
        }
    }
}
