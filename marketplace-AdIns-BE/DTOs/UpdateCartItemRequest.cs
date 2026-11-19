using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class UpdateCartItemRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity minimal 1")]
        public int Quantity { get; set; }
    }
}
