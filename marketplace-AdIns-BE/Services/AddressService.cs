using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;
using marketplace_AdIns_BE.DTOs;
using Microsoft.Extensions.Logging;

namespace marketplace_AdIns_BE.Services
{
    public class AddressService : IAddressService
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly ILogger<AddressService> _logger;

        public AddressService(
            IRepository<Address> addressRepository,
            ILogger<AddressService> logger)
        {
            _addressRepository = addressRepository;
            _logger = logger;
        }

        // 1. GET: Mengambil semua alamat berdasarkan UserId
        public async Task<List<AddressResponse>> GetAllAddressesByUserId(long userId)
        {
            // Ambil semua alamat (bisa difilter di database jika repository mendukung)
            var addresses = await _addressRepository.GetAllAsync();

            // Filter berdasarkan UserId
            var userAddresses = addresses.Where(a => a.UserId == userId).ToList();

            if (!userAddresses.Any()) return null;

            // Mapping ke DTO
            return userAddresses.Select(a => ToAddressResponse(a)).ToList();
        }

        // 2. GET: Mengambil satu alamat berdasarkan ID
        public async Task<AddressResponse?> GetAddressById(long addressId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null) return null;

            return ToAddressResponse(address);
        }

        // 3. POST: Membuat alamat baru
        public async Task<AddressResponse?> CreateAsync(AddressRequest request)
        {
            await _addressRepository.BeginTransactionAsync();
            try
            {
                // Validasi manual jika diperlukan (opsional, karena DTO sudah pakai [Required])
                if (string.IsNullOrEmpty(request.RecipientName) || string.IsNullOrEmpty(request.City))
                {
                    return null;
                }

                var newAddress = new Address
                {
                    UserId = request.UserId,
                    RecipientName = request.RecipientName,
                    PhoneNumber = request.PhoneNumber,
                    City = request.City,
                    PostalCode = request.PostalCode,
                    Note = request.Note,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _addressRepository.AddAsync(newAddress);
                await _addressRepository.SaveChangesAsync();
                await _addressRepository.CommitTransactionAsync();

                _logger.LogInformation($"Alamat baru berhasil dibuat untuk UserId: {newAddress.UserId}, AddressId: {newAddress.Id}");

                return ToAddressResponse(newAddress);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // 4. PUT: Mengubah alamat
        public async Task<AddressResponse?> UpdateAsync(long addressId, AddressRequest request)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null) return null;

            // Update properti
            address.RecipientName = request.RecipientName;
            address.PhoneNumber = request.PhoneNumber;
            address.City = request.City;
            address.PostalCode = request.PostalCode;
            address.Note = request.Note;
            address.UpdatedAt = DateTime.UtcNow;

            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();

            return ToAddressResponse(address);
        }

        // 5. DELETE: Menghapus alamat
        public async Task<bool> DeleteAsync(long addressId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null) return false;

            _addressRepository.Delete(address);
            await _addressRepository.SaveChangesAsync();

            return true;
        }

        // 6. MAPPER: Mengubah Entity ke DTO
        public AddressResponse ToAddressResponse(Address address)
        {
            return new AddressResponse
            {
                Id = address.Id,
                UserId = address.UserId,
                RecipientName = address.RecipientName,
                PhoneNumber = address.PhoneNumber,
                City = address.City,
                PostalCode = address.PostalCode,
                FullAddress = address.Note,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };
        }
    }
}