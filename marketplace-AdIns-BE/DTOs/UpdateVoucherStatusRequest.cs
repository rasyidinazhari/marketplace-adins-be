using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class UpdateVoucherStatusRequest
    {
        [Required]
        public bool? IsActive { get; set; }
    }
}
