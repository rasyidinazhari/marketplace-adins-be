using System.ComponentModel.DataAnnotations;
namespace marketplace_AdIns_BE.DTOs
{
    public class AddressRequest
    {
        [Required(ErrorMessage = "UserId Wajib Diisi")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "RecipientName Wajib Diisi")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "PhoneNumber Wajib Diisi")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City Wajib Diisi")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "PostalCode Wajib Diisi")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Note Wajib Diisi")]
        public string? Note { get; set; } = string.Empty;

    }
}
