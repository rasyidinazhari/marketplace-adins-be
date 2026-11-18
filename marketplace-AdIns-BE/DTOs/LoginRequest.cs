namespace marketplace_AdIns_BE.DTOs
{
    public class LoginRequest
    {
        public string Status { get; set; } = "success";
        public string Token { get; set; } = string.Empty;
        public UserSummary User { get; set; } = new();
    }
    public class UserSummary
    {
        public long Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}