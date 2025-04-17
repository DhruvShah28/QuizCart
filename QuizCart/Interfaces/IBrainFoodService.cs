using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Interfaces
{
    public interface IBrainFoodService
    {
        Task<IEnumerable<BrainFoodDto>> ListBrainFoods();
        Task<BrainFoodDto?> FindBrainFood(int id);
        Task<ServiceResponse> AddBrainFood(AddBrainFoodDto dto);
        Task<ServiceResponse> UpdateBrainFood(int id, UpdateBrainFoodDto dto);
        Task<ServiceResponse> DeleteBrainFood(int id);
        Task<PaginatedResult<BrainFoodDto>> GetPaginatedBrainFoods(int page, int pageSize);

    }
}
