using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class UpdateOrderStatusRequest
    {
        [Required(ErrorMessage = "Status wajib diisi")]
        public string Status { get; set; } = string.Empty;
    }
}
