using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Proccess;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comercializadora_de_pulpo_api.Controllers
{
    [Route("/process")]
    [ApiController]
    [Authorize]
    public class ProcessController(IProcessService processService) : ControllerBase
    {
        private readonly IProcessService _processService = processService;

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
        [Authorize(Policy = RoleAccess.MANAGERORWAREHOUSE)]
        public async Task<IActionResult> GetProcess([FromQuery] ProcessRequestDTO request)
        {
            return HandleResponse(await _processService.GetProcessAsync(request));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = RoleAccess.MANAGERORWAREHOUSE)]
        public async Task<IActionResult> GetProcessDetails([FromRoute] Guid id)
        {
            return HandleResponse(await _processService.GetProccessByIdAsync(id));
        }

        [HttpGet("production/{id:guid}")]
        [Authorize(Policy = RoleAccess.MANAGERORWAREHOUSE)]
        public async Task<IActionResult> GetProductionProcessDetails([FromRoute] Guid id)
        {
            return HandleResponse(await _processService.GetProductionProcessDetailsDTO(id));
        }

        [HttpPost]
        [Authorize(Policy = RoleAccess.ADMINORMANAGER)]
        public async Task<IActionResult> CreateProcess(CreateProcessDTO request)
        {
            return HandleResponse(await _processService.CreateProcessAsync(request));
        }

        [HttpPatch("{id:guid}/status/start")]
        [Authorize(Policy = RoleAccess.MANAGERORWAREHOUSE)]
        public async Task<IActionResult> StartProcess(Guid id)
        {
            string userId = Request.Headers["userId"].ToString();
            return HandleResponse(await _processService.StartProcessAsync(id, userId));
        }

        [HttpPatch("{id:guid}/status/end")]
        [Authorize(Policy = RoleAccess.MANAGERORWAREHOUSE)]
        public async Task<IActionResult> EndProcess(Guid id)
        {
            string userId = Request.Headers["userId"].ToString();
            return HandleResponse(await _processService.EndProcessAsync(id, userId));
        }
    }
}
