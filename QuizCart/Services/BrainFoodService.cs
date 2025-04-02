using Microsoft.EntityFrameworkCore;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Services
{
    public class BrainFoodService : IBrainFoodService
    {
        private readonly ApplicationDbContext _context;

        public BrainFoodService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BrainFoodDto>> ListBrainFoods()
        {
            var brainFoods = await _context.BrainFoods
                .Include(bf => bf.Assessment)
                .Include(bf => bf.Ingredient)
                .Include(bf => bf.Purchases)
                    .ThenInclude(p => p.Member)
                .ToListAsync();

            return brainFoods.Select(bf => new BrainFoodDto
            {
                BrainFoodId = bf.BrainFoodId,
                Quantity = bf.Quantity,
                IngredientId = bf.IngredientId,
                IngredientName = bf.Ingredient?.Name ?? "Unknown",
                AssessmentId = bf.AssessmentId,
                AssessmentName = bf.Assessment?.Title ?? "Unknown",
                Benefits = bf.Ingredient?.Benefits ?? "",
                UnitPrice = bf.Ingredient?.UnitPrice ?? 0f,
                Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                {
                    MemberName = p.Member?.Name ?? "Unknown",
                    DatePurchased = p.DatePurchased
                }).ToList() ?? new()
            }).ToList();
        }


        public async Task<BrainFoodDto?> FindBrainFood(int id)
        {
            var bf = await _context.BrainFoods
                .Include(b => b.Assessment)
                .Include(b => b.Ingredient)
                .Include(b => b.Purchases)
                    .ThenInclude(p => p.Member)
                .FirstOrDefaultAsync(b => b.BrainFoodId == id);

            if (bf == null) return null;

            return new BrainFoodDto
            {
                BrainFoodId = bf.BrainFoodId,
                Quantity = bf.Quantity,
                AssessmentId = bf.AssessmentId,
                AssessmentName = bf.Assessment?.Title ?? "Unknown",
                IngredientId = bf.IngredientId,
                IngredientName = bf.Ingredient?.Name ?? "Unknown",
                Benefits = bf.Ingredient?.Benefits ?? "",
                UnitPrice = bf.Ingredient?.UnitPrice ?? 0,
                Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                {
                    MemberName = p.Member.Name,
                    DatePurchased = p.DatePurchased
                }).ToList() ?? new()
            };
        }


        public async Task<ServiceResponse> AddBrainFood(AddBrainFoodDto dto)
        {
            ServiceResponse response = new();

            var brainFood = new BrainFood
            {
                Quantity = dto.Quantity,
                IngredientId = dto.IngredientId,
                AssessmentId = dto.AssessmentId
            };

            try
            {
                _context.BrainFoods.Add(brainFood);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = brainFood.BrainFoodId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding brain food.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UpdateBrainFood(int id, UpdateBrainFoodDto dto)
        {
            ServiceResponse response = new();

            if (id != dto.BrainFoodId)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("BrainFood ID mismatch.");
                return response;
            }

            var brainFood = await _context.BrainFoods.FindAsync(id);
            if (brainFood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("BrainFood not found.");
                return response;
            }

            brainFood.Quantity = dto.Quantity;
            brainFood.IngredientId = dto.IngredientId;
            brainFood.AssessmentId = dto.AssessmentId;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating brain food.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteBrainFood(int id)
        {
            ServiceResponse response = new();

            var bf = await _context.BrainFoods.FindAsync(id);
            if (bf == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("BrainFood not found.");
                return response;
            }

            try
            {
                _context.BrainFoods.Remove(bf);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting brain food.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }
    }
}
