using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/stores")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost] // POST /api/stores
        public async Task<IActionResult> CreateStore([FromBody] StoreRequest request)
        {
            if (request.UserId <= 0)
                return BadRequest(new { status = "error", message = "UserId tidak valid." });

            var created = await _storeService.CreateAsync(request);
            return Created($"/api/stores/{created.Id}", created);
        }
    }
}