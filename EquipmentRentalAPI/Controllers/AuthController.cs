using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using EquipmentRentalAPI.Models;
using EquipmentRentalAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EquipmentRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            Log.Information("Login attempt for email: {Email}", loginModel?.Email);

            if (loginModel == null || string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
            {
                Log.Warning("Login failed: Missing email or password.");
                return BadRequest("Email and password must be provided.");
            }

            var tokenDTO = await _authService.LoginAsync(loginModel);
            if (tokenDTO == null)
            {
                Log.Warning("Login failed for email: {Email}", loginModel.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            Log.Information("Login successful for email: {Email}", loginModel.Email);
            return Ok(tokenDTO);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            Log.Information("Registration attempt for email: {Email}", registerModel?.Email);

            if (registerModel.Password != registerModel.ConfirmPassword)
            {
                Log.Warning("Registration failed for email: {Email} - Passwords do not match.", registerModel.Email);
                return BadRequest("Entered passwords are different!");
            }

            if (registerModel.Password.Length < 8)
            {
                Log.Warning("Registration failed for email: {Email} - Password too short.", registerModel.Email);
                return BadRequest("Password is too short");
            }

            if (await _authService.UserExists(registerModel.Email))
            {
                Log.Warning("Registration failed for email: {Email} - Email already exists.", registerModel.Email);
                return BadRequest("Email already exists.");
            }

            if (!await _authService.RegisterAsync(registerModel))
            {
                Log.Error("Registration failed for email: {Email} - Unexpected error.", registerModel.Email);
                return BadRequest("Registration failed.");
            }

            Log.Information("Registration successful for email: {Email}", registerModel.Email);
            return Created();
        }
    }
}