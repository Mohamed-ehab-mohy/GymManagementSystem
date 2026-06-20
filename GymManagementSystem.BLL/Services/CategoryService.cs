using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository repository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task AddCategoryAsync(Category entity)
    {
        await _categoryRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateCategoryAsync(Category entity)
    {
        _categoryRepository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var entity = await _categoryRepository.GetByIdAsync(id);
        if (entity != null)
        {
            _categoryRepository.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
