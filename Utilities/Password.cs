using BCrypt.Net;

namespace comercializadora_de_pulpo_api.Utilities
{
    public class Password
    {
        private readonly IConfiguration _configuration;

        public Password(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Encrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordStored)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordStored);
        }

        //Hacer Verificación con Regex
    }
}
