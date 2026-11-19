using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace marketplace_AdIns_BE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IRepository<User> userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
        {
            List<User> users = await _userRepository.GetAllAsync("UserRole.Role", "Address", "Store");
            User user = users.FirstOrDefault(u => u.Email == loginRequest.Email);

            if (user == null || user.Password != loginRequest.Password) return null;

            bool hasMatchingRole = user.UserRoles.Any(ur => ur.Role != null && ur.Role.RoleName == loginRequest.Role);

            // Jika role tidak cocok, tolak login
            if (!hasMatchingRole) return null;

            //Create Payload
            Claim[] claims = new Claim[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.Name),
                new Claim("role", loginRequest.Role)
            };

            //Set Credetial
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Create Token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds
                );

            return new LoginResponse
            {
                Status = "success",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                User = new UserSummary
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = loginRequest.Role,
                }
            };
        }
    }
}
