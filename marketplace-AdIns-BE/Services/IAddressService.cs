using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
namespace marketplace_AdIns_BE.Services
{
    public interface IAddressService
    {
        public Task<List<AddressResponse>> GetAllAddressesByUserId(long userId);
        public Task<AddressResponse?> GetAddressById(long addressId);
        Task<AddressResponse?> CreateAsync(AddressRequest request);
        Task<AddressResponse?> UpdateAsync(long addressId, AddressRequest request);
        Task<bool> DeleteAsync(long addressId);
        AddressResponse ToAddressResponse(Address address);
    }
}
