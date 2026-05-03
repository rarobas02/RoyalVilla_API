using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        public Task<bool?> IsEmailExistAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
