namespace marketplace_AdIns_BE.DTOs
{
    public class StoreResponse
    {
        public long Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
