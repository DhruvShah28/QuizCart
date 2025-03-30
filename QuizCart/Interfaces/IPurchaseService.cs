using QuizCart.Models;

namespace QuizCart.Interfaces
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchasesDto>> ListPurchases();
        Task<PurchasesDto?> FindPurchase(int id);
        Task<ServiceResponse> AddPurchase(AddPurchasesDto dto);
        Task<ServiceResponse> UpdatePurchase(int id, UpdatePurchasesDto dto);
        Task<ServiceResponse> DeletePurchase(int id);
        Task<ServiceResponse> LinkBrainFood(LinkBrainFoodDto dto);
        Task<ServiceResponse> UnlinkBrainFood(LinkBrainFoodDto dto);
        Task<IEnumerable<PurchasesDto>> ListPurchasesByMemberId(int memberId);

        Task<ServiceResponse> AddPurchaseWithBrainFood(AddPurchasesDto dto, BrainFood brainFood);


    }
}
