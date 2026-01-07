using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Reports;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/reports")]
    [ApiController]
    [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
    public class ReportsController(IReportService reportService) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;

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

        [HttpGet("sales")]
        public async Task<IActionResult> SalesReport([FromQuery] ReportRequestDTO request)
        {
            return HandleResponse(await _reportService.GetSaleReportAsync(request));
        }

        [HttpGet("clients")]
        public async Task<IActionResult> ClientsReport([FromQuery] ReportRequestDTO request)
        {
            return HandleResponse(await _reportService.GetClientReportAsync(request));
        }

        [HttpGet("products")]
        public async Task<IActionResult> ProductsReport([FromQuery] ReportRequestDTO request)
        {
            return HandleResponse(await _reportService.GetProductReportAsync(request));
        }

        [HttpGet("supplies")]
        public async Task<IActionResult> SuppliesReport([FromQuery] ReportRequestDTO request)
        {
            return HandleResponse(await _reportService.GetSuppliesReportAsync(request));
        }

        [HttpGet("purchases")]
        public async Task<IActionResult> PurchasesReport([FromQuery] ReportRequestDTO request)
        {
            return HandleResponse(await _reportService.GetPurchaseReportAsync(request));
        }
    }
}
