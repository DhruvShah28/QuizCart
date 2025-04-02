using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Controllers
{
    [Route("BrainFoodsPage")]
    public class BrainFoodsPageController : Controller
    {
        private readonly IBrainFoodService _brainFoodService;
        private readonly IIngredientService _ingredientService;
        private readonly IAssessmentService _assessmentService;

        public BrainFoodsPageController(IBrainFoodService brainFoodService, IIngredientService ingredientService, IAssessmentService assessmentService)
        {
            _brainFoodService = brainFoodService;
            _ingredientService = ingredientService;
            _assessmentService = assessmentService;
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction("List");

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var brainFoods = await _brainFoodService.ListBrainFoods();
            return View(brainFoods);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var brainFood = await _brainFoodService.FindBrainFood(id);
            if (brainFood == null)
                return View("Error", new ErrorViewModel { Errors = ["BrainFood not found."] });

            return View(brainFood);
        }

        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Ingredients = new SelectList(await _ingredientService.ListIngredients(), "IngredientId", "Name");
            ViewBag.Assessments = new SelectList(await _assessmentService.ListAssessments(), "AssessmentId", "Title");
            return View();
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddBrainFoodDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Ingredients = new SelectList(await _ingredientService.ListIngredients(), "IngredientId", "Name");
                ViewBag.Assessments = new SelectList(await _assessmentService.ListAssessments(), "AssessmentId", "Title");
                return View(dto);
            }

            var result = await _brainFoodService.AddBrainFood(dto);
            if (result.Status != ServiceResponse.ServiceStatus.Created)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("List");
        }

        [HttpGet("Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var brainFood = await _brainFoodService.FindBrainFood(id);
            if (brainFood == null)
                return View("Error", new ErrorViewModel { Errors = ["BrainFood not found."] });

            var dto = new UpdateBrainFoodDto
            {
                BrainFoodId = brainFood.BrainFoodId,
                Quantity = brainFood.Quantity,
                IngredientId = 0, // You may need to extend DTO to return IngredientId
                AssessmentId = 0
            };

            ViewBag.Ingredients = new SelectList(await _ingredientService.ListIngredients(), "IngredientId", "Name");
            ViewBag.Assessments = new SelectList(await _assessmentService.ListAssessments(), "AssessmentId", "Title");
            return View(dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, UpdateBrainFoodDto dto)
        {
            if (id != dto.BrainFoodId)
                return View("Error", new ErrorViewModel { Errors = ["BrainFood ID mismatch."] });

            var result = await _brainFoodService.UpdateBrainFood(id, dto);
            if (result.Status == ServiceResponse.ServiceStatus.Error)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("Details", new { id });
        }

        [HttpGet("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var brainFood = await _brainFoodService.FindBrainFood(id);
            if (brainFood == null)
                return View("Error", new ErrorViewModel { Errors = ["BrainFood not found."] });

            return View(brainFood);
        }

        [HttpPost("Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brainFoodService.DeleteBrainFood(id);
            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
                return RedirectToAction("List");

            return View("Error", new ErrorViewModel { Errors = result.Messages });
        }
    }
}
