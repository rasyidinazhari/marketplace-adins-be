using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Repositories;

namespace marketplace_AdIns_BE.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<List<Category>> GetAllAsync()
        {
            return _categoryRepository.GetAllAsync();
        }
    }
}
