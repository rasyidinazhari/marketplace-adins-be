using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace marketplace_AdIns_BE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IConfiguration _config;

        public AuthService(
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<Role> roleRepository,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
        {
            List<User> users = await _userRepository.GetAllAsync("UserRoles.Role", "Addresses", "Stores");

            User? user = users.FirstOrDefault(u =>
                !string.IsNullOrWhiteSpace(u.Email) &&
                u.Email.Equals(loginRequest.Email?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                u.UserRoles.Any(ur => ur.Role != null && ur.Role.RoleName == loginRequest.Role)
            );

            if (user == null) return null;

            if (user.Password != loginRequest.Password) return null;

            // Create Payload
            Claim[] claims = new Claim[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.Name),
                new Claim(ClaimTypes.Role, loginRequest.Role)
            };

            // Set Credential
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            double expireMinutes = double.TryParse(_config["Jwt:ExpireMinutes"], out var parsedMinutes)
                ? parsedMinutes
                : 60;

            // Create Token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
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

        public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
        {
            // 1. Buka Transaksi
            await _userRepository.BeginTransactionAsync();

            try
            {
                var normalizedEmail = request.Email.Trim().ToLower();
                var normalizedRole = request.Role.Trim();

                // 2. Cek apakah role valid di tabel Roles
                var roles = await _roleRepository.GetAllAsync();
                var roleEntity = roles.FirstOrDefault(r =>
                    !string.IsNullOrWhiteSpace(r.RoleName) &&
                    r.RoleName.Equals(normalizedRole, StringComparison.OrdinalIgnoreCase));

                if (roleEntity == null)
                {
                    await _userRepository.RollbackTransactionAsync();
                    return null; // Role tidak valid
                }

                // 3. Cek duplikasi email untuk role yang sama
                // Menggunakan method Repository agar efisien
                var users = await _userRepository.GetAllAsync("UserRoles.Role");
                var userExisting = users.FirstOrDefault(u =>
                    !string.IsNullOrWhiteSpace(u.Email) &&
                    u.Email.Trim().ToLower() == normalizedEmail &&
                    u.UserRoles.Any(ur => ur.Role != null &&
                                          ur.Role.RoleName.Equals(roleEntity.RoleName, StringComparison.OrdinalIgnoreCase)));

                Console.WriteLine("userExisting ", userExisting);

                if (userExisting != null)
                {
                    await _userRepository.RollbackTransactionAsync();
                    return null; // Email sudah terdaftar
                }

                // 4. Simpan User baru
                var user = new User
                {
                    Name = request.Name.Trim(),
                    Email = normalizedEmail,
                    Password = request.Password, 
                    isActive = true,
                    Created_at = DateTime.UtcNow,
                    Updated_at = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync(); // Simpan dulu untuk mendapatkan User.Id

                // 5. Hubungkan User dengan Role melalui UserRole
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleEntity.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRoleRepository.AddAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();

                // 6. Commit Transaksi
                await _userRepository.CommitTransactionAsync();

                // 7. Return Response yang sesuai
                return new RegisterResponse
                {
                    Status = "success",
                    Message = "Registrasi berhasil.",
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = roleEntity.RoleName ?? string.Empty,
                    IsActive = user.isActive,
                    CreatedAt = user.Created_at
                };
            }
            catch (Exception)
            {
                await _userRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(long userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }
    }
}