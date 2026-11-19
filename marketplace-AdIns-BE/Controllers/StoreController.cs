using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.Services;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/stores")]
    public class StoreController : ControllerBase
    {
       /* private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet] // GET /api/stores
        public async Task<IActionResult> GetStores()
        {
            var stores = await _storeService.GetAllAsync();
            return Ok(stores);
        }

        [HttpGet("{id:long}")] // GET /api/stores/{id}
        public async Task<IActionResult> GetStore(long id)
        {
            var store = await _storeService.GetByIdAsync(id);
            if (store == null) return NotFound();
            return Ok(store);
        }

        [Authorize]
        [HttpPost] // POST /api/stores
        public async Task<IActionResult> CreateStores([FromBody] StoreRequest request)
        {
            var created = await _storeService.CreateAsync(request);
            return Created($"/api/stores/{created.Id}", created);
        }

        [Authorize]
        [HttpPut("{id:long}")] // PUT /api/stores/{id}
        public async Task<IActionResult> UpdateStore(long id, [FromBody] StoreRequest request)
        {
            var updated = await _storeService.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        [Authorize]
        [HttpDelete("{id:long}")] // DELETE /api/stores/{id}
        public async Task<IActionResult> DeleteStore(long id)
        {
            var result = await _storeService.DeleteAsync(id);
            if (result == null) return NotFound();
            if (result == false)
                return Conflict(new { error = "Store ada transaksi yang sedang berjalan" });
            return NoContent();
        }*/

    }
}
