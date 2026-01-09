using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Dashboard;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardDTO> GetDashboardByUserAsync(User user);
    }
}
