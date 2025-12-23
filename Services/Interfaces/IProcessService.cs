using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Proccess;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IProcessService
    {
        Task<Response<ProcessResponseDTO>> GetProcessAsync(ProcessRequestDTO request);
        Task<Response<ProcessDetailsDTO>> GetProccessByIdAsync(Guid processId);
        Task<Response<ProductionProcessDetailsDTO>> GetProductionProcessDetailsDTO(Guid processId);
        Task<Response<ProcessDTO>> CreateProcessAsync(CreateProcessDTO request);
        Task<Response<ProcessDTO>> StartProcessAsync(Guid processId, string? userId);
        Task<Response<ProcessDTO>> EndProcessAsync(Guid processId, string? userId);
    }
}
