using System.Security.Claims;
using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Login;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;

namespace comercializadora_de_pulpo_api.Services
{
    public class AuthService(
        IUserRepository userRepository,
        JWTHelper jwtHelper,
        Password password,
        IMapper mapper
    ) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly JWTHelper _jwtHelper = jwtHelper;
        private readonly Password _password = password;
        private readonly IMapper _mapper = mapper;

        private readonly int accessTokenDurationMinutes = 10;
        private readonly int refreshTokenDurationHours = 12;

        public async Task<Response<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
        {
            var userSaved = await _userRepository.GetUserByEmailAsync(request.Email);

            if (userSaved == null || userSaved.IsDeleted)
                return Response<LoginResponseDTO>.Fail(
                    "No se encontró una cuenta activa vinculada al correo",
                    $"No se encontró una cuenta activa con el correo '{request.Email}'",
                    401
                );

            if (!_password.Verify(request.Password, userSaved.Password))
                return Response<LoginResponseDTO>.Fail(
                    "Correo o contraseña incorrectas",
                    "El correo o contraseña proporcionados son incorrectos",
                    401
                );

            var response = _mapper.Map<LoginResponseDTO>(userSaved);

            response.AccessToken = _jwtHelper.GenerateAccessToken(
                userSaved,
                accessTokenDurationMinutes
            );
            response.AccessTokenExpiratesAt = DateTime.UtcNow.AddMinutes(
                accessTokenDurationMinutes
            );
            response.RefreshToken = _jwtHelper.GenerateRefreshToken(
                userSaved,
                refreshTokenDurationHours
            );
            response.RefreshTokenExpiresAt = DateTime.UtcNow.AddHours(refreshTokenDurationHours);
            response.FirstLogin = userSaved.FirstLogin;

            return Response<LoginResponseDTO>.Ok(response);
        }

        public async Task<Response<RefreshTokenResponseDTO>> RefreshAccessTokenAsync(
            RefreshTokenRequestDTO request
        )
        {
            var principal = _jwtHelper.GetPrincipalFromToken(request.RefreshToken);

            if (principal == null || principal.FindFirst(ClaimTypes.NameIdentifier)?.Value == null)
            {
                return Response<RefreshTokenResponseDTO>.Fail(
                    "Token Inválido",
                    "El token proporcionado es inválido",
                    401
                );
            }

            Guid userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var userResponse = await _userRepository.GetUserByIdAsync(userId);

            if (userResponse == null || userResponse.IsDeleted)
                return Response<RefreshTokenResponseDTO>.Fail(
                    "Token Inválido",
                    "El token proporcionado es inválido",
                    401
                );

            String newAccessToken = _jwtHelper.GenerateAccessToken(
                userResponse,
                accessTokenDurationMinutes
            );

            RefreshTokenResponseDTO data = new()
            {
                Token = newAccessToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenDurationMinutes),
            };

            return Response<RefreshTokenResponseDTO>.Ok(data);
        }
    }
}
