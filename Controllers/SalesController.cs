using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Sale;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/sales")]
    [Authorize]
    [ApiController]
    public class SalesController(ISalesService salesService) : ControllerBase
    {
        private readonly ISalesService _salesService = salesService;

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
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> GetSalesAsync([FromQuery] SalesRequestDTO request)
        {
            return HandleResponse(await _salesService.GetSalesAsync(request));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> GetSaleDetailsByIdAsync([FromRoute] Guid id)
        {
            return HandleResponse(await _salesService.GetSaleDetailsByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> MakeSale([FromBody] SaleRequest request)
        {
            return HandleResponse(await _salesService.SaveSale(request));
        }
    }
}
