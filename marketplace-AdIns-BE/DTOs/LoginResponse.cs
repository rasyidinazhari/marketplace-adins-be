namespace marketplace_AdIns_BE.DTOs
{
    public class LoginResponse
    {
        public string Status { get; set; } = "success";
        public string Token { get; set; } = string.Empty;
        public UserSummary User { get; set; } = new();
    }

    
}
