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
        /// <summary>
        /// Redirects to the List action.
        /// </summary>


        [HttpGet]
        public IActionResult Index() => RedirectToAction("List");

        /// <summary>
        /// Displays a list of all brain food items.
        /// </summary>
        /// <returns>View with list of brain foods.</returns>
        /// <example>GET: BrainFoodsPage/List</example>

        [HttpGet("List")]
        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var result = await _brainFoodService.GetPaginatedBrainFoods(page, pageSize);
            return View(result);
        }


        /// <summary>
        /// Displays details of a specific brain food item.
        /// </summary>
        /// <param name="id">ID of the brain food.</param>
        /// <returns>View with details or error view.</returns>
        /// <example>GET: BrainFoodsPage/Details/1</example>

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var brainFood = await _brainFoodService.FindBrainFood(id);
            if (brainFood == null)
                return View("Error", new ErrorViewModel { Errors = ["BrainFood not found."] });

            return View(brainFood);
        }

        /// <summary>
        /// Displays the form to add a new brain food item.
        /// </summary>
        /// <returns>View with dropdowns for ingredients and assessments.</returns>


        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Ingredients = new SelectList(await _ingredientService.ListIngredients(), "IngredientId", "Name");
            ViewBag.Assessments = new SelectList(await _assessmentService.ListAssessments(), "AssessmentId", "Title");
            return View();
        }

        /// <summary>
        /// Submits the form to add a new brain food item.
        /// </summary>
        /// <param name="dto">AddBrainFoodDto containing the data.</param>
        /// <returns>Redirect to list or error view.</returns>


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

        /// <summary>
        /// Displays the form to edit a brain food item.
        /// </summary>
        /// <param name="id">ID of the brain food to edit.</param>
        /// <returns>View with pre-filled data or error view.</returns>


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

        /// <summary>
        /// Submits the form to update a brain food item.
        /// </summary>
        /// <param name="id">ID of the brain food to update.</param>
        /// <param name="dto">Updated values for the brain food.</param>
        /// <returns>Redirect to details or error view.</returns>


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

        /// <summary>
        /// Shows a confirmation page before deleting a brain food item.
        /// </summary>
        /// <param name="id">ID of the brain food.</param>
        /// <returns>Confirmation view or error view.</returns>


        [HttpGet("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var brainFood = await _brainFoodService.FindBrainFood(id);
            if (brainFood == null)
                return View("Error", new ErrorViewModel { Errors = ["BrainFood not found."] });

            return View(brainFood);
        }


        /// <summary>
        /// Deletes a brain food item.
        /// </summary>
        /// <param name="id">ID of the brain food to delete.</param>
        /// <returns>Redirect to list or error view.</returns>


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
