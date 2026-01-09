using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Dashboard;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;

namespace comercializadora_de_pulpo_api.Services
{
    public class DashboardService(
        IDashboardRepository dashboardRepository,
        IUserRepository userRepository
    ):IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository = dashboardRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Response<DashboardDTO>> GetDashboardAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                return Response<DashboardDTO>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{userId}'",
                    404
                );

            var dashboard = await _dashboardRepository.GetDashboardByUserAsync(user);

            return Response<DashboardDTO>.Ok(dashboard);
        }
    }
}
