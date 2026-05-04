using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;
using RoyalVilla_API.Services;

namespace RoyalVilla_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService; //use primary constructor to inject the auth service if only one class to inject is needed

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                //validate the request
                if (registrationRequestDTO == null)
                {
                    var errorResponse = ApiResponse<UserDTO>.BadRequest("Registration Data is Required");
                    return BadRequest(errorResponse);
                }
                //check if email already exists
                if (await _authService.IsEmailExistAsync(registrationRequestDTO.Email))
                {
                    var errorResponse = ApiResponse<UserDTO>.Conflict($"User with email {registrationRequestDTO.Email} already exists");
                    return Conflict(errorResponse);
                }
                var user = await _authService.RegisterAsync(registrationRequestDTO);
                //auth service
                if (user == null)
                {
                    return BadRequest(ApiResponse<UserDTO>.BadRequest("User Registration failed"));
                }
                var response = ApiResponse<UserDTO>.Ok(user, "Users Registered successfully");
                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, "An Error occurred during registration", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
