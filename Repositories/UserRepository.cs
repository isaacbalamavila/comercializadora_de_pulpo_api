using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.User;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class UserRepository(ComercializadoraDePulpoContext context, IMapper mapper)
        : IUserRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<List<User>> GetUsersAsync(Guid userId, bool all)
        {
            var query = _context
                .Users.Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .AsQueryable();

            if (!all)
                query = query.Where(u => u.Id != userId);

            return await query.ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context
                .Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context
                .Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> VerifyEmailAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> VerifyPhoneAsync(string phone)
        {
            return !await _context.Users.AnyAsync(u => u.Phone == phone);
        }

        public async Task<Response<User>> CreateUserAsync(User newUser)
        {
            try
            {
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return Response<User>.Ok(newUser);
            }
            catch (Exception ex)
            {
                return Response<User>.Fail("Error al intentar crear el usuario", ex.Message);
            }
        }

        public async Task<Response<User>> UpdateUserAsync(User updatedUser)
        {
            try
            {
                _context.Users.Update(updatedUser);

                await _context.SaveChangesAsync();

                return Response<User>.Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return Response<User>.Fail(
                    "Error al intentantar actualizar el usuario",
                    ex.Message
                );
            }
        }
    }
}
