using Microsoft.EntityFrameworkCore;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseService(ApplicationDbContext context)
        {
            _context = context;
        }

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
                IngredientNames = p.BrainFoods.Select(bf => bf.Ingredient.Name).Distinct().ToList()
            }).ToList();
        }

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
                IngredientNames = p.BrainFoods.Select(bf => bf.Ingredient.Name).Distinct().ToList()
            };
        }

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
                IngredientNames = p.BrainFoods.Select(bf => bf.Ingredient.Name).ToList()
            }).ToList();
        }
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


    }
}
