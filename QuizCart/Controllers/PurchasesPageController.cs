using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IBrainFoodService _brainFoodService;
        private readonly IAssessmentService _assessmentService;
        private readonly IIngredientService _ingredientService;


        public PurchasesPageController(IIngredientService ingredientService, IAssessmentService assessmentService, IPurchaseService purchaseService, IMemberService memberService, IBrainFoodService brainFoodService)
        {
            _purchaseService = purchaseService;
            _memberService = memberService;
            _brainFoodService = brainFoodService;
            _assessmentService = assessmentService;
            _ingredientService = ingredientService;
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
                DatePurchased = DateOnly.FromDateTime(DateTime.Today),
                Members = (await _memberService.ListMembers())
                    .Select(m => new SelectListItem { Text = m.Name, Value = m.MemberId.ToString() })
                    .ToList(),
                Assessments = (await _assessmentService.ListAssessments())
                    .Select(a => new SelectListItem { Text = a.Title, Value = a.AssessmentId.ToString() })
                    .ToList(),
                Ingredients = (await _ingredientService.ListIngredients())
                    .Select(i => new SelectListItem { Text = i.Name, Value = i.IngredientId.ToString() })
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
                // repopulate dropdowns
                vm.Members = (await _memberService.ListMembers())
                    .Select(m => new SelectListItem { Text = m.Name, Value = m.MemberId.ToString() })
                    .ToList();
                vm.Assessments = (await _assessmentService.ListAssessments())
                    .Select(a => new SelectListItem { Text = a.Title, Value = a.AssessmentId.ToString() })
                    .ToList();
                vm.Ingredients = (await _ingredientService.ListIngredients())
                    .Select(i => new SelectListItem { Text = i.Name, Value = i.IngredientId.ToString() })
                    .ToList();

                return View(vm);
            }

            // create brain food from assessment, ingredient, and quantity
            var brainFood = new BrainFood
            {
                AssessmentId = vm.AssessmentId,
                IngredientId = vm.IngredientId,
                Quantity = vm.Quantity
            };

            var dto = new AddPurchasesDto
            {
                MemberId = vm.MemberId,
                DatePurchased = vm.DatePurchased,
                BrainFoodIds = new List<int>()
            };

            var result = await _purchaseService.AddPurchaseWithBrainFood(dto, brainFood);

            if (result.Status != ServiceResponse.ServiceStatus.Created)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("List");
        }

        [HttpGet("EditPurchase/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _purchaseService.FindPurchaseForEdit(id);
            if (purchase == null)
                return View("Error", new ErrorViewModel { Errors = ["Purchase not found."] });

            var brainFood = purchase.BrainFoods?.FirstOrDefault();
            if (brainFood == null)
                return View("Error", new ErrorViewModel { Errors = ["No BrainFood entry found for this purchase."] });

            var vm = new UpdatePurchaseViewModel
            {
                PurchaseId = purchase.PurchaseId,
                DatePurchased = purchase.DatePurchased,
                MemberId = purchase.MemberId,
                AssessmentId = brainFood.AssessmentId,
                IngredientId = brainFood.IngredientId,
                Quantity = brainFood.Quantity,

                Members = (await _memberService.ListMembers())
                    .Select(m => new SelectListItem { Text = m.Name, Value = m.MemberId.ToString() })
                    .ToList(),
                Assessments = (await _assessmentService.ListAssessments())
                    .Select(a => new SelectListItem { Text = a.Title, Value = a.AssessmentId.ToString() })
                    .ToList(),
                Ingredients = (await _ingredientService.ListIngredients())
                    .Select(i => new SelectListItem { Text = i.Name, Value = i.IngredientId.ToString() })
                    .ToList()
            };

            return View(vm);
        }



        [HttpPost("EditPurchase/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePurchaseViewModel vm)
        {
            if (id != vm.PurchaseId)
                return View("Error", new ErrorViewModel { Errors = ["Purchase ID mismatch."] });

            if (!ModelState.IsValid)
            {
                vm.Members = (await _memberService.ListMembers())
                    .Select(m => new SelectListItem { Text = m.Name, Value = m.MemberId.ToString() })
                    .ToList();
                vm.Assessments = (await _assessmentService.ListAssessments())
                    .Select(a => new SelectListItem { Text = a.Title, Value = a.AssessmentId.ToString() })
                    .ToList();
                vm.Ingredients = (await _ingredientService.ListIngredients())
                    .Select(i => new SelectListItem { Text = i.Name, Value = i.IngredientId.ToString() })
                    .ToList();

                return View(vm);
            }

            var brainFood = new BrainFood
            {
                AssessmentId = vm.AssessmentId,
                IngredientId = vm.IngredientId,
                Quantity = vm.Quantity
            };

            var dto = new UpdatePurchasesDto
            {
                PurchaseId = vm.PurchaseId,
                DatePurchased = vm.DatePurchased,
                BrainFoodIds = new List<int>() // will be added/linked via service
            };

            var result = await _purchaseService.UpdatePurchaseWithBrainFood(dto, brainFood);

            if (result.Status != ServiceResponse.ServiceStatus.Updated)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("List");
        }



        [HttpGet("DeletePurchase/{id}")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);
            if (purchase == null)
                return View("Error", new ErrorViewModel { Errors = ["Purchase not found."] });

            return View(purchase);
        }

        [HttpPost("DeletePurchase/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _purchaseService.DeletePurchase(id);
            return result.Status == ServiceResponse.ServiceStatus.Deleted
                ? RedirectToAction("List")
                : View("Error", new ErrorViewModel { Errors = result.Messages });
        }
    }
}
