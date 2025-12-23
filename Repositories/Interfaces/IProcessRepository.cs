using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Proccess;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface IProcessRepository
    {
        Task<Response<ProcessResponseDTO>> GetProcessesAsync(ProcessRequestDTO request);
        Task<ProductionProcess?> GetProcessByIdAsync(Guid processId);
        Task<ProductionProcessDetailsDTO?> GetProductionProcessDetails(Guid processId);
        Task<Response<ProductionProcess>> UpdateProcessAsync(ProductionProcess processUpdated);
    }
}
