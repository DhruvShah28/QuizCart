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

        /// <summary>
        /// Retrieves a list of all ingredients, each including the details of brain foods,
        /// related assessments, and associated purchases by members.
        /// </summary>
        /// <returns>
        /// A list of IngredientDto containing ingredient information, the assessments it is used in,
        /// and which members purchased items containing this ingredient.
        /// </returns>

        public async Task<IEnumerable<IngredientDto>> ListIngredients()
        {
            var ingredients = await _context.Ingredients
                .Include(i => i.BrainFoods!)
                    .ThenInclude(bf => bf.Assessment)
                .Include(i => i.BrainFoods!)
                    .ThenInclude(bf => bf.Purchases!)
                        .ThenInclude(p => p.Member)
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
                    .Count(),
                AssessmentsUsedIn = i.BrainFoods.Select(bf => new BrainFoodDto
                {
                    BrainFoodId = bf.BrainFoodId,
                    AssessmentId = bf.AssessmentId,
                    AssessmentName = bf.Assessment?.Title ?? "Unknown",
                    Quantity = bf.Quantity,
                    UnitPrice = i.UnitPrice,
                    Benefits = i.Benefits,
                    IngredientId = i.IngredientId,
                    IngredientName = i.Name,
                    Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                    {
                        MemberName = p.Member?.Name ?? "Unknown",
                        DatePurchased = p.DatePurchased
                    }).ToList() ?? new()
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Retrieves a specific ingredient by ID, including its associated assessments and purchases.
        /// </summary>
        /// <param name="id">The ID of the ingredient.</param>
        /// <returns>
        /// An IngredientDto if found, otherwise null.
        /// </returns>

        public async Task<IngredientDto?> FindIngredient(int id)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.BrainFoods!)
                    .ThenInclude(bf => bf.Assessment)
                .Include(i => i.BrainFoods!)
                    .ThenInclude(bf => bf.Purchases!)
                        .ThenInclude(p => p.Member)
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
                    .Count(),
                AssessmentsUsedIn = ingredient.BrainFoods.Select(bf => new BrainFoodDto
                {
                    BrainFoodId = bf.BrainFoodId,
                    AssessmentId = bf.AssessmentId,
                    AssessmentName = bf.Assessment?.Title ?? "Unknown",
                    Quantity = bf.Quantity,
                    UnitPrice = ingredient.UnitPrice,
                    Benefits = ingredient.Benefits,
                    IngredientId = ingredient.IngredientId,
                    IngredientName = ingredient.Name,
                    Purchases = bf.Purchases?.Select(p => new BrainFoodPurchaseDto
                    {
                        MemberName = p.Member?.Name ?? "Unknown",
                        DatePurchased = p.DatePurchased
                    }).ToList() ?? new()
                }).ToList()
            };
        }

        /// <summary>
        /// Adds a new ingredient to the database.
        /// </summary>
        /// <param name="dto">The data transfer object containing ingredient details.</param>
        /// <returns>
        /// A ServiceResponse with Created status if successful, or Error status with messages if not.
        /// </returns>

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

        /// <summary>
        /// Updates an existing ingredient based on the provided ID and data.
        /// </summary>
        /// <param name="id">The ID of the ingredient to update.</param>
        /// <param name="dto">The updated ingredient information.</param>
        /// <returns>
        /// A ServiceResponse indicating whether the update was successful or failed.
        /// </returns>

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

        /// <summary>
        /// Deletes an ingredient by its ID.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete.</param>
        /// <returns>
        /// A ServiceResponse indicating the result of the delete operation.
        /// </returns>

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
