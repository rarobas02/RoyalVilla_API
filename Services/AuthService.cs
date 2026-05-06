using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public AuthService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()); //if any user with the same email exists in the database, it will return true, otherwise false
            //alternatively we can use CurrentCultureIgnoreCase to ignore case sensitivity
            //return await _db.Users.AnyAsync(u => u.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)); //does not work due string.equals cannot translate to string comparison to sql
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            try 
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower() && u.Password == loginRequestDTO.Password); //find the user with the same email and password, if not found return null 
                if (user == null || user.Password != loginRequestDTO.Password)
                {
                    return null;
                }

                //Generate JWT token for the user
                return new LoginResponseDTO
                {
                    UserDTO = _mapper.Map<UserDTO>(user), //this will convert the User to a UserDTO and return it
                    Token = ""
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while logging in the user.", ex);
            }
        }

        public async Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                if (await IsEmailExistAsync(registrationRequestDTO.Email))
                {
                    throw new InvalidOperationException($"User with email {registrationRequestDTO.Email} already exists"); //email already exists, return null to indicate registration failed
                }
                User user = new() //this will convert the RegistrationRequestDTO to a User
                {
                    Email = registrationRequestDTO.Email,
                    Name = registrationRequestDTO.Name,
                    Password = registrationRequestDTO.Password,
                    Role = string.IsNullOrEmpty(registrationRequestDTO.Role) ? "Customer" : registrationRequestDTO.Role, //if role is not provided, default to "Customer",
                    CreatedDate = DateTime.UtcNow
                };
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                return _mapper.Map<UserDTO>(user); //this will convert the User to a UserDTO and return it
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while registering the user.", ex);
            }
        }
    }
}
