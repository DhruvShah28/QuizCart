using Microsoft.EntityFrameworkCore;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all purchases including member, brainfoods, and calculated totals.
        /// </summary>
        /// <returns>List of PurchasesDto containing purchase details with associated brainfoods and total amount.</returns>

        public async Task<IEnumerable<PurchasesDto>> ListPurchases()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Member)
                .Include(p => p.BrainFoods)
                    .ThenInclude(bf => bf.Ingredient)
                .ToListAsync();

            return purchases.Select(p => new PurchasesDto
            {
                PurchaseId = p.PurchaseId,
                DatePurchased = p.DatePurchased,
                MemberName = p.Member.Name,
                TotalAmount = p.BrainFoods.Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice),
                IngredientNames = p.BrainFoods.Select(bf => bf.Ingredient.Name).Distinct().ToList(),
                Items = p.BrainFoods.Select(bf => new PurchaseItemDto
                {
                    IngredientName = bf.Ingredient.Name,
                    Quantity = bf.Quantity,
                    UnitPrice = bf.Ingredient.UnitPrice
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Retrieves a specific purchase by ID with all associated data.
        /// </summary>
        /// <param name="id">The ID of the purchase.</param>
        /// <returns>A PurchasesDto or null if not found.</returns>

        public async Task<PurchasesDto?> FindPurchase(int id)
        {
            var p = await _context.Purchases
                .Include(p => p.Member)
                .Include(p => p.BrainFoods)
                    .ThenInclude(bf => bf.Ingredient)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (p == null) return null;

            return new PurchasesDto
            {
                PurchaseId = p.PurchaseId,
                DatePurchased = p.DatePurchased,
                MemberName = p.Member.Name,
                TotalAmount = p.BrainFoods.Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice),
                IngredientNames = p.BrainFoods.Select(bf => bf.Ingredient.Name).Distinct().ToList(),
                Items = p.BrainFoods.Select(bf => new PurchaseItemDto
                {
                    IngredientName = bf.Ingredient.Name,
                    Quantity = bf.Quantity,
                    UnitPrice = bf.Ingredient.UnitPrice
                }).ToList()
            };
        }

        /// <summary>
        /// Adds a new purchase and links associated brain food items.
        /// </summary>
        /// <param name="dto">AddPurchasesDto with member ID, date, and brainfood IDs.</param>
        /// <returns>ServiceResponse with creation status or error message.</returns>

        public async Task<ServiceResponse> AddPurchase(AddPurchasesDto dto)
        {
            ServiceResponse response = new();

            var member = await _context.Members.FindAsync(dto.MemberId);
            if (member == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Member not found.");
                return response;
            }

            var purchase = new Purchase
            {
                DatePurchased = dto.DatePurchased,
                MemberId = dto.MemberId,
                BrainFoods = new List<BrainFood>()
            };

            try
            {
                foreach (var bfId in dto.BrainFoodIds)
                {
                    var brainFood = await _context.BrainFoods.FindAsync(bfId);
                    if (brainFood != null)
                    {
                        purchase.BrainFoods.Add(brainFood);
                    }
                }

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = purchase.PurchaseId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding purchase.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Updates an existing purchase and its linked brainfood items.
        /// </summary>
        /// <param name="id">The ID of the purchase to update.</param>
        /// <param name="dto">UpdatePurchasesDto with new brainfood links.</param>
        /// <returns>ServiceResponse indicating update status or errors.</returns>

        public async Task<ServiceResponse> UpdatePurchase(int id, UpdatePurchasesDto dto)
        {
            var response = new ServiceResponse();

            var purchase = await _context.Purchases
                .Include(p => p.BrainFoods)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Purchase not found.");
                return response;
            }

            purchase.DatePurchased = dto.DatePurchased;

            purchase.BrainFoods.Clear();

            var brainFoods = await _context.BrainFoods
                .Where(bf => dto.BrainFoodIds.Contains(bf.BrainFoodId))
                .ToListAsync();

            foreach (var bf in brainFoods)
            {
                purchase.BrainFoods.Add(bf);
            }

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating purchase.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Deletes a purchase by ID.
        /// </summary>
        /// <param name="id">The ID of the purchase to delete.</param>
        /// <returns>ServiceResponse indicating deletion result.</returns>


        public async Task<ServiceResponse> DeletePurchase(int id)
        {
            ServiceResponse response = new();

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Purchase not found.");
                return response;
            }

            try
            {
                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting purchase.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Links a brainfood item to an existing purchase.
        /// </summary>

        public async Task<ServiceResponse> LinkBrainFood(LinkBrainFoodDto dto)
        {
            var response = new ServiceResponse();

            var purchase = await _context.Purchases
                .Include(p => p.BrainFoods)
                .FirstOrDefaultAsync(p => p.PurchaseId == dto.PurchaseId);

            var brainFood = await _context.BrainFoods.FindAsync(dto.BrainFoodId);

            if (purchase == null || brainFood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Purchase or BrainFood not found.");
                return response;
            }

            if (!purchase.BrainFoods.Contains(brainFood))
            {
                purchase.BrainFoods.Add(brainFood);
                await _context.SaveChangesAsync();
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }
        /// <summary>
        /// Unlinks a brainfood item from an existing purchase.
        /// </summary>

        public async Task<ServiceResponse> UnlinkBrainFood(LinkBrainFoodDto dto)
        {
            var response = new ServiceResponse();

            var purchase = await _context.Purchases
                .Include(p => p.BrainFoods)
                .FirstOrDefaultAsync(p => p.PurchaseId == dto.PurchaseId);

            if (purchase == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Purchase not found.");
                return response;
            }

            var brainFood = purchase.BrainFoods.FirstOrDefault(bf => bf.BrainFoodId == dto.BrainFoodId);
            if (brainFood == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("BrainFood not linked to purchase.");
                return response;
            }

            purchase.BrainFoods.Remove(brainFood);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        /// <summary>
        /// Lists all purchases made by a specific member.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>List of PurchasesDto for the specified member.</returns>

        public async Task<IEnumerable<PurchasesDto>> ListPurchasesByMemberId(int memberId)
        {
            var purchases = await _context.Purchases
                .Where(p => p.MemberId == memberId)
                .Include(p => p.BrainFoods)
                    .ThenInclude(bf => bf.Ingredient)
                .Include(p => p.Member)
                .ToListAsync();

            return purchases.Select(p => new PurchasesDto
            {
                PurchaseId = p.PurchaseId,
                DatePurchased = p.DatePurchased,
                MemberName = p.Member.Name,
                TotalAmount = p.BrainFoods.Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice),
                IngredientNames = p.BrainFoods.Select(bf => bf.Ingredient.Name).Distinct().ToList(),
                Items = p.BrainFoods.Select(bf => new PurchaseItemDto
                {
                    IngredientName = bf.Ingredient.Name,
                    Quantity = bf.Quantity,
                    UnitPrice = bf.Ingredient.UnitPrice
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Adds a new purchase and also creates a new BrainFood entry to associate.
        /// </summary>

        public async Task<ServiceResponse> AddPurchaseWithBrainFood(AddPurchasesDto dto, BrainFood brainFood)
        {
            var response = new ServiceResponse();

            try
            {
                var member = await _context.Members.FindAsync(dto.MemberId);
                if (member == null)
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("Member not found.");
                    return response;
                }

                var ingredient = await _context.Ingredients.FindAsync(brainFood.IngredientId);
                if (ingredient == null)
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("Ingredient not found.");
                    return response;
                }

                brainFood.Ingredient = ingredient;

                var purchase = new Purchase
                {
                    DatePurchased = dto.DatePurchased,
                    MemberId = dto.MemberId,
                    BrainFoods = new List<BrainFood> { brainFood }
                };

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = purchase.PurchaseId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding purchase.");
                response.Messages.Add(ex.Message);
                if (ex.InnerException != null)
                    response.Messages.Add(ex.InnerException.Message);
            }

            return response;
        }

        /// <summary>
        /// Updates an existing purchase and creates a new associated BrainFood item.
        /// </summary>

        public async Task<ServiceResponse> UpdatePurchaseWithBrainFood(UpdatePurchasesDto dto, BrainFood updatedBrainFood)
        {
            var response = new ServiceResponse();

            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.BrainFoods)
                    .FirstOrDefaultAsync(p => p.PurchaseId == dto.PurchaseId);

                if (purchase == null)
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("Purchase not found.");
                    return response;
                }

                purchase.BrainFoods.Clear();

                var ingredientExists = await _context.Ingredients.AnyAsync(i => i.IngredientId == updatedBrainFood.IngredientId);
                var assessmentExists = await _context.Assessments.AnyAsync(a => a.AssessmentId == updatedBrainFood.AssessmentId);

                if (!ingredientExists || !assessmentExists)
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("Ingredient or Assessment not found.");
                    return response;
                }

              
                var newBrainFood = new BrainFood
                {
                    Quantity = updatedBrainFood.Quantity,
                    IngredientId = updatedBrainFood.IngredientId,
                    AssessmentId = updatedBrainFood.AssessmentId
                };

                _context.BrainFoods.Add(newBrainFood);
                await _context.SaveChangesAsync(); 

                purchase.DatePurchased = dto.DatePurchased;
                purchase.BrainFoods.Add(newBrainFood);

                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating purchase.");
                response.Messages.Add(ex.Message);
                if (ex.InnerException != null)
                    response.Messages.Add(ex.InnerException.Message);
            }

            return response;
        }

        /// <summary>
        /// Returns a raw Purchase model for edit purposes with all linked brainfoods.
        /// </summary>
        /// <param name="id">Purchase ID</param>
        /// <returns>Purchase entity with linked BrainFoods or null</returns>


        public async Task<Purchase?> FindPurchaseForEdit(int id)
        {
            return await _context.Purchases
                .Include(p => p.BrainFoods)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
        }

        public async Task<PaginatedResult<PurchasesDto>> GetPaginatedPurchases(int page, int pageSize)
        {
            var query = _context.Purchases
                .Include(p => p.Member)
                .Include(p => p.BrainFoods)
                    .ThenInclude(bf => bf.Ingredient)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var purchases = await query
                .OrderByDescending(p => p.DatePurchased)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = purchases.Select(p => new PurchasesDto
            {
                PurchaseId = p.PurchaseId,
                DatePurchased = p.DatePurchased,
                MemberName = p.Member.Name,
                TotalAmount = p.BrainFoods.Sum(bf => bf.Quantity * bf.Ingredient.UnitPrice),
                IngredientNames = p.BrainFoods.Select(bf => bf.Ingredient.Name).Distinct().ToList(),
                Items = p.BrainFoods.Select(bf => new PurchaseItemDto
                {
                    IngredientName = bf.Ingredient.Name,
                    Quantity = bf.Quantity,
                    UnitPrice = bf.Ingredient.UnitPrice
                }).ToList()
            }).ToList();

            return new PaginatedResult<PurchasesDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

    }
}