using marketplace_AdIns_BE.Models;
using Microsoft.AspNetCore.Mvc;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAdrressService scheduleService)
        {
            _addressService = scheduleService;
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetAddrress(long id)
        {
            var addresses = await ) _addressService.GetByIdAsync(id);
            if (addresses == null) returdn NotFound();
            return Ok(addresses);
        }


    }
}
