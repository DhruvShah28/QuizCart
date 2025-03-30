using QuizCart.Models;

namespace QuizCart.Interfaces
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDto>> ListIngredients();
        Task<IngredientDto?> FindIngredient(int id);
        Task<ServiceResponse> AddIngredient(AddIngredientDto dto);
        Task<ServiceResponse> UpdateIngredient(int id, UpdateIngredientDto dto);
        Task<ServiceResponse> DeleteIngredient(int id);
    }
}
