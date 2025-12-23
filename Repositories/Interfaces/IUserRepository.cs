using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.User;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<List<User>> GetUsersAsync(Guid userId, bool all);
        Task<bool> VerifyEmailAsync(string email);
        Task<bool> VerifyPhoneAsync(string phone);

        Task<Response<User>> CreateUserAsync(User newUser);
        Task<Response<User>> UpdateUserAsync(User updateUser);
    }
}
