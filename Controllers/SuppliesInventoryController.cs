using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/supplies-inventory")]
    [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
    [ApiController]
    public class SuppliesInventoryController (ISuppliesInventoryService suppliesService) : ControllerBase
    {
        private readonly ISuppliesInventoryService _suppliesService = suppliesService;

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
        public async Task<IActionResult> GetSuppliesInventory([FromQuery]SuppliesRequestDTO request) {
            return HandleResponse(await _suppliesService.GetSuppliesAsync(request));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSupplyDetails(Guid id) {
            return HandleResponse(await _suppliesService.GetSupplyByIdAsync(id));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateWeightRemain(Guid id, [FromBody] UpdateWeightRemain request) {
            return HandleResponse(await _suppliesService.UpdateWeightRemainAsync(id, request));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DisposeSupply(Guid id) {
            return HandleResponse(await _suppliesService.DisposeSupplyAsync(id));
        }
    }
}
