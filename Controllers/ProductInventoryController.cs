using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/product-inventory")]
    [Authorize]
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
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> GetBatchesInventory([FromQuery]ProductBatchesRequestDTO request) {
            return HandleResponse(await _productInventoryService.GetProductBatchInventoryAsync(request));
        }

        [HttpGet("batches/{id:guid}")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> GetProductBatchDetails(Guid id) {
            return HandleResponse(await _productInventoryService.GetProductDetailsByIdAsync(id));
        }

        [HttpPatch("batches/{id:guid}")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> UpdateBatchRemain(Guid id, [FromBody] UpdateRemain request) {
            return HandleResponse(await _productInventoryService.UpdateBatchRemain(id,request));
        }

        [HttpDelete("batches/{id:guid}")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> DisposeProductBatch(Guid id)
        {
            return HandleResponse(await _productInventoryService.DisposeProductBatchByIdAsync(id));
        }
    }
}
