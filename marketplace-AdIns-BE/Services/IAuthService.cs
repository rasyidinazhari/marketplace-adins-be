using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<RegisterResponse?> RegisterAsync(RegisterRequest request);
        Task<User?> GetByIdAsync(long userId);
    }
}
