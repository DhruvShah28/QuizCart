using Microsoft.EntityFrameworkCore;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IngredientDto>> ListIngredients()
        {
            var ingredients = await _context.Ingredients
                .Include(i => i.BrainFoods)
                    .ThenInclude(bf => bf.Assessment)
                .ToListAsync();

            return ingredients.Select(i => new IngredientDto
            {
                IngredientId = i.IngredientId,
                Name = i.Name,
                Benefits = i.Benefits,
                UnitPrice = i.UnitPrice,
                TotalAssessments = i.BrainFoods
                    .Select(bf => bf.AssessmentId)
                    .Distinct()
                    .Count()
            }).ToList();
        }

        public async Task<IngredientDto?> FindIngredient(int id)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.BrainFoods)
                    .ThenInclude(bf => bf.Assessment)
                .FirstOrDefaultAsync(i => i.IngredientId == id);

            if (ingredient == null) return null;

            return new IngredientDto
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                Benefits = ingredient.Benefits,
                UnitPrice = ingredient.UnitPrice,
                TotalAssessments = ingredient.BrainFoods
                    .Select(bf => bf.AssessmentId)
                    .Distinct()
                    .Count()
            };
        }

        public async Task<ServiceResponse> AddIngredient(AddIngredientDto dto)
        {
            ServiceResponse response = new();

            var ingredient = new Ingredient
            {
                Name = dto.Name,
                Benefits = dto.Benefits,
                UnitPrice = dto.UnitPrice
            };

            try
            {
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = ingredient.IngredientId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding ingredient.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UpdateIngredient(int id, UpdateIngredientDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.IngredientId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Ingredient ID mismatch.");
                return response;
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Ingredient not found.");
                return response;
            }

            ingredient.Name = dto.Name;
            ingredient.Benefits = dto.Benefits;
            ingredient.UnitPrice = dto.UnitPrice;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating ingredient.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteIngredient(int id)
        {
            ServiceResponse response = new();

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Ingredient not found.");
                return response;
            }

            try
            {
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting ingredient.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }
    }
}
