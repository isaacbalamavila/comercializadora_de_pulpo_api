using System.Data.Entity.Migrations.Model;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.User;

namespace comercializadora_de_pulpo_api.Services.Interfaces
{
    public interface IUserService
    {
        Task<Response<List<UserDTO>>> GetUsersAsync(string userId, bool all);
        Task<Response<UserDTO>> GetUserByIdAsync(Guid userId);
        Task<Response<UserDetailsDTO>> GetUserDetailsByIdAsync(Guid userId);
        Task<Response<UserDTO>> CreateUserAsync(CreateUserDTO request);
        Task<Response<UserDetailsDTO>> UpdateUserAsync(Guid userId, UpdateUserDTO request);

        Task<Response<bool>> ChangePasswordAsync(Guid userId, string newPassword);
        Task<Response<bool>> ResetPasswordAsync(Guid userId);

        Task<Response<bool>> DeleteUserAsync(Guid userId);
        Task<Response<bool>> RestoreUserAsync(Guid userId);
    }
}
