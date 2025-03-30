using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;
using QuizCart.Models.ViewModels;

namespace QuizCart.Controllers
{
    [Route("IngredientsPage")]
    public class IngredientsPageController : Controller
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsPageController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var ingredients = await _ingredientService.ListIngredients();
            return View(ingredients);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);
            if (ingredient == null)
                return View("Error", new ErrorViewModel { Errors = ["Ingredient not found."] });

            return View(ingredient);
        }

        [HttpGet("AddIngredient")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("AddIngredient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddIngredientDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _ingredientService.AddIngredient(dto);
            if (result.Status == ServiceResponse.ServiceStatus.Error)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("List");
        }

        [HttpGet("EditIngredient/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);
            if (ingredient == null)
                return View("Error", new ErrorViewModel { Errors = ["Ingredient not found."] });

            var dto = new UpdateIngredientDto
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                Benefits = ingredient.Benefits,
                UnitPrice = ingredient.UnitPrice
            };

            return View(dto);
        }

        [HttpPost("EditIngredient/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateIngredientDto dto)
        {
            if (id != dto.IngredientId)
                return View("Error", new ErrorViewModel { Errors = ["Ingredient ID mismatch."] });

            var result = await _ingredientService.UpdateIngredient(id, dto);
            if (result.Status == ServiceResponse.ServiceStatus.Error)
                return View("Error", new ErrorViewModel { Errors = result.Messages });

            return RedirectToAction("Details", new { id });
        }

        [HttpGet("DeleteIngredient/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);
            if (ingredient == null)
                return View("Error", new ErrorViewModel { Errors = ["Ingredient not found."] });

            return View(ingredient);
        }

        [HttpPost("DeleteIngredient/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ingredientService.DeleteIngredient(id);
            if (result.Status == ServiceResponse.ServiceStatus.Deleted)
                return RedirectToAction("List");

            return View("Error", new ErrorViewModel { Errors = result.Messages });
        }
    }
}
