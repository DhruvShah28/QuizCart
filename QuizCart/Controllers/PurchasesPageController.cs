using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;
using QuizCart.Services;

namespace QuizCart.Controllers
{
    [Route("PurchasesPage")]
    public class PurchasesPageController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IMemberService _memberService;
        private readonly IIngredientService _ingredientService;
        private readonly IAssessmentService _assessmentService;

        public PurchasesPageController(IAssessmentService assessmentService, IPurchaseService purchaseService,  IMemberService memberService, IIngredientService ingredientService)
        {
            _purchaseService = purchaseService;
            _ingredientService = ingredientService;
            _memberService = memberService;
            _assessmentService = assessmentService;
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction("List");

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var purchases = await _purchaseService.ListPurchases();
            return View(purchases);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);
            if (purchase == null)
                return View("Error", new ErrorViewModel { Errors = ["Purchase not found."] });

            return View(purchase);
        }

        [HttpGet("AddPurchase")]
        public async Task<IActionResult> Add()
        {
            var vm = new AddPurchaseViewModel
            {
                Members = (await _memberService.ListMembers())
                            .Select(m => new SelectListItem { Text = m.Name, Value = m.MemberId.ToString() })
                            .ToList(),
                Ingredients = (await _ingredientService.ListIngredients())
                            .Select(i => new SelectListItem { Text = i.Name, Value = i.IngredientId.ToString() })
                            .ToList(),
                Assessments = (await _assessmentService.ListAssessments())
                            .Select(a => new SelectListItem { Text = a.Title, Value = a.AssessmentId.ToString() })
                            .ToList()
            };

            return View(vm);
        }


        [HttpPost("AddPurchase")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddPurchaseViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Members = (await _memberService.ListMembers())
                            .Select(m => new SelectListItem { Text = m.Name, Value = m.MemberId.ToString() })
                            .ToList();

                vm.Ingredients = (await _ingredientService.ListIngredients())
                            .Select(i => new SelectListItem { Text = i.Name, Value = i.IngredientId.ToString() })
                            .ToList();

                vm.Assessments = (await _assessmentService.ListAssessments())
                            .Select(a => new SelectListItem { Text = a.Title, Value = a.AssessmentId.ToString() })
                            .ToList();

                return View(vm); 
            }

            try
            {
                var dto = new AddPurchasesDto
                {
                    MemberId = vm.MemberId,
                    DatePurchased = vm.DatePurchased,
                    BrainFoodIds = new List<int>() 
                };

                var brainFood = new BrainFood
                {
                    IngredientId = vm.IngredientId,
                    AssessmentId = vm.AssessmentId, 
                    Quantity = vm.Quantity
                };

                var result = await _purchaseService.AddPurchaseWithBrainFood(dto, brainFood);

                if (result.Status != ServiceResponse.ServiceStatus.Created)
                {
                    return View("Error", new ErrorViewModel { Errors = result.Messages });
                }

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string>
            {
                "An unexpected error occurred.",
                ex.Message,
                ex.InnerException?.Message ?? "",
                ex.StackTrace ?? ""
            }
                });
            }
        }


        [HttpGet("DeletePurchase/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);
            if (purchase == null)
                return View("Error", new ErrorViewModel { Errors = ["Purchase not found."] });

            return View(purchase);
        }

        [HttpPost("DeletePurchase/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _purchaseService.DeletePurchase(id);
            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
                return RedirectToAction("List");

            return View("Error", new ErrorViewModel { Errors = result.Messages });
        }
    }
}
