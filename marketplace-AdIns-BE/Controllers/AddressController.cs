using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Authorize] // Wajib login untuk mengakses alamat
    [Route("api/address")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // 1. GET: Ambil semua alamat milik user yang sedang login
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            // Ambil UserId dari Token JWT
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var addresses = await _addressService.GetAllAddressesByUserId(userId);
            return Ok(addresses);
        }

        // 2. GET: Ambil detail satu alamat berdasarkan ID
        [Authorize]
        [HttpGet("{addressId:long}")]
        public async Task<IActionResult> GetAddressById(long addressId)
        {
            var address = await _addressService.GetAddressById(addressId);

            if (address == null) return NotFound();

            // Validasi tambahan: Pastikan alamat ini milik user yang sedang login
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out long userId) || address.UserId != userId)
            {
                return Forbid("Anda tidak memiliki akses ke alamat ini.");
            }

            return Ok(address);
        }

        // 3. POST: Tambah alamat baru
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressRequest request)
        {
            // Ambil UserId dari Token JWT (Abaikan UserId dari body request)
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            // Timpa UserId di request dengan UserId dari Token
            request.UserId = userId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAddress = await _addressService.CreateAsync(request);
            if (createdAddress == null) return BadRequest("Gagal membuat alamat.");

            return CreatedAtAction(nameof(GetAddressById), new { addressId = createdAddress.Id }, createdAddress);
        }

        // 4. PUT: Ubah alamat
        [Authorize]
        [HttpPut("{addressId:long}")]
        public async Task<IActionResult> UpdateAddress(long addressId, [FromBody] AddressRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            // Cek apakah alamat ada dan milik user ini
            var existingAddress = await _addressService.GetAddressById(addressId);
            if (existingAddress == null) return NotFound();
            if (existingAddress.UserId != userId) return Forbid("Anda tidak memiliki akses ke alamat ini.");

            // Timpa UserId di request
            request.UserId = userId;

            var updatedAddress = await _addressService.UpdateAsync(addressId, request);
            if (updatedAddress == null) return BadRequest("Gagal mengubah alamat.");

            return Ok(updatedAddress);
        }

        // 5. DELETE: Hapus alamat
        [Authorize]
        [HttpDelete("{addressId:long}")]
        public async Task<IActionResult> DeleteAddress(long addressId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            // Cek apakah alamat ada dan milik user ini
            var existingAddress = await _addressService.GetAddressById(addressId);
            if (existingAddress == null) return NotFound();
            if (existingAddress.UserId != userId) return Forbid("Anda tidak memiliki akses ke alamat ini.");

            bool removed = await _addressService.DeleteAsync(addressId);
            if (!removed) return BadRequest("Gagal menghapus alamat.");

            return NoContent();
        }
    }
}