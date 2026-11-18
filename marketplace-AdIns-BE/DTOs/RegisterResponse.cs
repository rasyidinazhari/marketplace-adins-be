namespace marketplace_AdIns_BE.DTOs
{
    public class RegisterResponse
    {
        public string Status { get; set; } = "success";
        public string Message { get; set; } = "Registrasi berhasil.";
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}