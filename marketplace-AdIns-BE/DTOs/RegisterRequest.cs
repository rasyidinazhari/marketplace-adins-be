using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Nama wajib diisi.")]
        [MaxLength(100, ErrorMessage = "Nama maksimal 100 karakter.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [MaxLength(150, ErrorMessage = "Email maksimal 150 karakter.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi.")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
        [MaxLength(255, ErrorMessage = "Password maksimal 255 karakter.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role wajib diisi.")]
        [MaxLength(30, ErrorMessage = "Role maksimal 30 karakter.")]
        public string Role { get; set; } = "Buyer"; // Dipetakan ke Role.RoleName melalui tabel UserRole
    }
}