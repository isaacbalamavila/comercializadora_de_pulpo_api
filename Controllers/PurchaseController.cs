using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Purchases;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/purchases")]
    [ApiController]
    [Authorize]
    public class PurchaseController (IPurchaseService purchaseService) : ControllerBase
    {
        private readonly IPurchaseService _purchaseService = purchaseService;
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
        public async Task<IActionResult> GetPurchases([FromQuery] PurchaseRequestDTO request) {
            return HandleResponse(await _purchaseService.GetPurchasesAsync(request));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> GetPurchasesDetails(Guid id)
        {
            return HandleResponse(await _purchaseService.GetPurchaseByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchase(CreatePurchaseDTO request) {
            return HandleResponse(await _purchaseService.CreatePurchase(request));
        }


    }
}
