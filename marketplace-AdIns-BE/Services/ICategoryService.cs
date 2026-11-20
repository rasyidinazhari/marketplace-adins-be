using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
    }
}
