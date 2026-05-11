using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoyalVilla.DTO;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoyalVilla_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthService(ApplicationDbContext db, IConfiguration configuration, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
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
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower()); //find the user with the same email and password, if not found return null 
                if (user == null || user.Password != loginRequestDTO.Password)
                {
                    return null;
                }

                //Generate JWT token for the user
                var token = GenerateJwtToken(user);
                return new LoginResponseDTO
                {
                    UserDTO = _mapper.Map<UserDTO>(user), //this will convert the User to a UserDTO and return it
                    Token = token
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
                throw new InvalidOperationException("An error occurred while registering the user.", ex);
            }
        }
        public string GenerateJwtToken(User user)
        {
            
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["SecretKey"]); // convert to byte array to use as the secret key for signing the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddDays(7), //which means the token will expire in 7 days
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  //validate if the token is valid by using the same secret key and algorithm that was used to generate the token
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor); // this will create a JWT token based on the token descriptor which contains the claims, expiration and signing credentials
            return tokenHandler.WriteToken(token); //this will return the token as a string
        }
    }
}
