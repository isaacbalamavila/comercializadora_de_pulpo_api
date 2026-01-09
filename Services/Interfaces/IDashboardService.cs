using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Dashboard;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<Response<DashboardDTO>> GetDashboardAsync(Guid userId);
    }
}
