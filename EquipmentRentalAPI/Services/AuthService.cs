using System.Security.Cryptography;
using System.Text;
using EquipmentRentalAPI.DTO;
using EquipmentRentalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EquipmentRentalAPI.Services
{
    public interface IAuthService
    {
        Task<TokenDTO?> LoginAsync(LoginModel loginModel);
        Task<bool> RegisterAsync(RegisterModel registerModel);
        Task<bool> UserExists(string email);
    }

    public class AuthService : IAuthService
    {
        private readonly EquipmentRentalsContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(EquipmentRentalsContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<TokenDTO?> LoginAsync(LoginModel loginModel)
        {
            Log.Information("Attempting login for user with email: {Email}", loginModel.Email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null || !VerifyPassword(loginModel.Password, user.PasswordHash))
            {
                Log.Warning("Login failed for user with email: {Email}. User not found or incorrect password.", loginModel.Email);
                return null;
            }

            Log.Information("Login successful for user with email: {Email}", loginModel.Email);

            var tokenDTO = new TokenDTO
            {
                Token = _jwtTokenService.GenerateToken(user),
                Role = user.Role
            };

            return tokenDTO;
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            Log.Information("Attempting to register user with email: {Email}", model.Email);

            if (await UserExists(model.Email))
            {
                Log.Warning("Registration failed. User with email {Email} already exists.", model.Email);
                return false; // User already exists
            }

            string hashedPassword = HashPassword(model.Password);
            var user = new User
            {
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = "Client"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Log.Information("User with email {Email} registered successfully.", model.Email);
            return true;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
                return hash == storedHash;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
