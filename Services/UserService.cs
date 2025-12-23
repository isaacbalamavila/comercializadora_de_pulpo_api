using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.User;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;

namespace comercializadora_de_pulpo_api.Services
{
    public class UserService(
        IUserRepository userRepository,
        IRolesRepository roleRepository,
        IMapper mapper,
        Password password
    ) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRolesRepository _roleRepository = roleRepository;
        private readonly IMapper _mapper = mapper;
        private readonly Password _password = password;

        public async Task<Response<List<UserDTO>>> GetUsersAsync(string userId, bool all)
        {
            if (!all & String.IsNullOrEmpty(userId))
                return Response<List<UserDTO>>.Fail(
                    "El Encabezado userID es obligatorio.",
                    "No se proporcionó un ID válido en el header 'userID'.",
                    400
                );

            var users = await _userRepository.GetUsersAsync(Guid.Parse(userId), all);

            return users == null
                ? Response<List<UserDTO>>.Fail(
                    "El usuario que realizó la petición no existe.",
                    "No se proporcionó un ID válido en el header 'userID'.",
                    400
                )
                : Response<List<UserDTO>>.Ok(_mapper.Map<List<UserDTO>>(users));
        }

        public async Task<Response<UserDTO>> GetUserByIdAsync(Guid userId)
        {
            var userSaved = await _userRepository.GetUserByIdAsync(userId);

            return userSaved == null
                ? Response<UserDTO>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró ningún usuario con el ID '{userId}'.",
                    404
                )
                : Response<UserDTO>.Ok(_mapper.Map<UserDTO>(_mapper.Map<UserDTO>(userSaved)));
        }

        public async Task<Response<UserDetailsDTO>> GetUserDetailsByIdAsync(Guid userId)
        {
            var userSaved = await _userRepository.GetUserByIdAsync(userId);

            return userSaved == null
                ? Response<UserDetailsDTO>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró ningún usuario con el ID '{userId}'.",
                    404
                )
                : Response<UserDetailsDTO>.Ok(_mapper.Map<UserDetailsDTO>(userSaved));
        }

        public async Task<Response<UserDTO>> CreateUserAsync(CreateUserDTO request)
        {
            request.Name = request.Name.Trim().ToLower();
            request.Email = request.Email.Trim().ToLower();
            request.LastName = request.LastName.Trim().ToLower();

            if (!await _userRepository.VerifyEmailAsync(request.Email))
                return Response<UserDTO>.Fail(
                    "Correo electrónico ya registrado",
                    $"El correo '{request.Email}' ya existe en el sistema.",
                    400
                );

            if (request.Phone != null && !await _userRepository.VerifyPhoneAsync(request.Phone))
                return Response<UserDTO>.Fail(
                    "Número de Teléfono ya registrado",
                    $"El número '{request.Phone}' ya existe en el sistema."
                );

            var role = await _roleRepository.GetRoleById(request.RoleId);

            if (role == null)
                return Response<UserDTO>.Fail(
                    "El rol no existe",
                    $"No se encontró un rol con el ID '{request.RoleId}' en el sistema."
                );

            var newUser = _mapper.Map<User>(request);
            newUser.Id = Guid.NewGuid();
            newUser.Password = _password.Encrypt(GenerateDefaultPassword());
            newUser.CreatedAt = DateTime.Now;
            newUser.Role = role;
            newUser.FirstLogin = true;

            var createRequest = await _userRepository.CreateUserAsync(newUser);

            return createRequest.IsSuccess
                ? Response<UserDTO>.Ok(_mapper.Map<UserDTO>(createRequest.Data), 201)
                : Response<UserDTO>.Fail(
                    "Ocurrió un error al intentar crear el usuario",
                    createRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<UserDetailsDTO>> UpdateUserAsync(
            Guid userId,
            UpdateUserDTO request
        )
        {
            request.Name = request.Name.Trim().ToLower();
            request.Email = request.Email.Trim().ToLower();
            request.LastName = request.LastName.Trim().ToLower();

            var userSaved = await _userRepository.GetUserByIdAsync(userId);

            if (userSaved == null)
                return Response<UserDetailsDTO>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{userId}'",
                    404
                );

            if (userSaved.IsDeleted)
                return Response<UserDetailsDTO>.Fail(
                    "No se puede modificar un usuario eliminado",
                    $"El usuario con el ID '{userId}' se encuentra elimnado y con las modificaciones restringidas",
                    400
                );

            if (
                !userSaved.Email.Equals(request.Email, StringComparison.CurrentCultureIgnoreCase)
                && !await _userRepository.VerifyEmailAsync(request.Email)
            )
                return Response<UserDetailsDTO>.Fail(
                    "Correo electrónico ya registrado",
                    $"El correo '{request.Email}' ya existe en el sistema.",
                    400
                );

            if (
                request.Phone != null
                && userSaved.Phone != request.Phone
                && !await _userRepository.VerifyPhoneAsync(request.Phone)
            )
                return Response<UserDetailsDTO>.Fail(
                    "Número de Teléfono ya registrado",
                    $"El número '{request.Phone}' ya existe en el sistema."
                );

            // Update Fields
            userSaved.Email = request.Email;
            userSaved.Name = request.Name;
            userSaved.LastName = request.LastName;
            userSaved.Phone = request.Phone;

            var updateRequest = await _userRepository.UpdateUserAsync(userSaved);

            return updateRequest.IsSuccess
                ? Response<UserDetailsDTO>.Ok(_mapper.Map<UserDetailsDTO>(updateRequest.Data))
                : Response<UserDetailsDTO>.Fail(
                    "Ocurrió un error al intentar actualizar el usuario",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<bool>> ChangePasswordAsync(Guid userId, string newPassword)
        {
            var savedUser = await _userRepository.GetUserByIdAsync(userId);

            if (savedUser == null)
                return Response<bool>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{userId}'",
                    404
                );

            if (savedUser.IsDeleted)
                return Response<bool>.Fail(
                    "No se puede modificar un usuario eliminado",
                    $"El usuario con el ID '{userId}' se encuentra elimnado y con las modificaciones restringidas",
                    400
                );

            savedUser.Password = _password.Encrypt(newPassword);
            savedUser.FirstLogin = false;
            var updateRequest = await _userRepository.UpdateUserAsync(savedUser);

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true, 200)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar actualizar la contraseña",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<bool>> ResetPasswordAsync(Guid userId)
        {
            var savedUser = await _userRepository.GetUserByIdAsync(userId);

            if (savedUser == null)
                return Response<bool>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{userId}'",
                    404
                );

            if (savedUser.IsDeleted)
                return Response<bool>.Fail(
                    "No se puede modificar un usuario eliminado",
                    $"El usuario con el ID '{userId}' se encuentra elimnado y con las modificaciones restringidas",
                    400
                );

            savedUser.Password = _password.Encrypt(GenerateDefaultPassword());
            savedUser.FirstLogin = true;

            var updateRequest = await _userRepository.UpdateUserAsync(savedUser);

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar restablecer la contraseña",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<bool>> DeleteUserAsync(Guid userId)
        {
            var savedUser = await _userRepository.GetUserByIdAsync(userId);

            if (savedUser == null)
                return Response<bool>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{userId}'",
                    404
                );

            if (savedUser.IsDeleted)
                return Response<bool>.Ok(true, 204);

            savedUser.IsDeleted = true;

            var updateRequest = await _userRepository.UpdateUserAsync(savedUser);

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true, 204)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar eliminar el usuario",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<bool>> RestoreUserAsync(Guid userId)
        {
            var savedUser = await _userRepository.GetUserByIdAsync(userId);

            if (savedUser == null)
                return Response<bool>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{userId}'",
                    404
                );

            if (!savedUser.IsDeleted)
                return Response<bool>.Ok(true);

            savedUser.IsDeleted = false;

            var updateRequest = await _userRepository.UpdateUserAsync(savedUser);

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar restaurar el usuario",
                    updateRequest.Error!.ErrorDetails
                );
        }

        private static string GenerateDefaultPassword()
        {
            return $"Comercializadora{DateTime.Now.Year}";
        }
    }
}
