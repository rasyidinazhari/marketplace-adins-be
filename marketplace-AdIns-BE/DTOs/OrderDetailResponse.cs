namespace marketplace_AdIns_BE.DTOs
{
    public class OrderDetailResponse : OrderResponse
    {
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
        public string PhoneNumber { get; set; } = string.Empty;
        public string? FullAddress { get; set; }
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}
