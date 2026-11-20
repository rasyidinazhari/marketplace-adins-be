namespace marketplace_AdIns_BE.DTOs
{
    public class OrderDetailResponse : OrderResponse
    {
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }
}
