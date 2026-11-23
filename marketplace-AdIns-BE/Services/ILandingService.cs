using marketplace_AdIns_BE.DTOs;

namespace marketplace_AdIns_BE.Services
{
    public interface ILandingService
    {
        Task<LandingStatsResponse> GetStatsAsync();
    }
}
