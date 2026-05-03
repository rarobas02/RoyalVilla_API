using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public async Task<bool?> IsEmailExistAsync(string email)
        {
            //return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()); //if any user with the same email exists in the database, it will return true, otherwise false
            //alternatively we can use CurrentCultureIgnoreCase to ignore case sensitivity
            return await _db.Users.AnyAsync(u => u.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
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
