using marketplace_AdIns_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/landing")]
    public class LandingController : ControllerBase
    {
        private readonly ILandingService _landingService;

        public LandingController(ILandingService landingService)
        {
            _landingService = landingService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _landingService.GetStatsAsync();
            return Ok(stats);
        }
    }
}
