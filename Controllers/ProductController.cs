using System.Text.Json;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/products")]
    [Authorize]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

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
        public async Task<IActionResult> GetAllProducts([FromQuery] bool? onlyActives)
        {
            return HandleResponse(await _productService.GetProductsAsync(onlyActives ?? false));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductDetails([FromRoute] Guid id)
        {
            return HandleResponse(await _productService.GetProductByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDTO request)
        {
            if (request == null)
            {
                return BadRequest("El cuerpo es Obligatorio");
            }

            return HandleResponse(await _productService.CreateProductAsync(request));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> UpdateProduct(
            [FromRoute] Guid id,
            [FromForm] UpdateProductDTO request
        )
        {
            return HandleResponse(await _productService.UpdateProductAsync(id, request));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            return HandleResponse(await _productService.DeleteProductAsync(id));
        }

        [HttpPatch("{id:guid}/restore")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> RestoreProduct([FromRoute] Guid id)
        {
            return HandleResponse(await _productService.RestoreProductAsync(id));
        }
    }
}
