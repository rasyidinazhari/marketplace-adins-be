using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class CartRequest
    {
        [Required(ErrorMessage = "UserId Wajib Diisi")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "ProductId Wajib Diisi")]
        public long ProductId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Quantity Minimal 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage ="TotalPrice Wajib Diisi")]
        public decimal TotalPrice { get; set; }
    }
}
