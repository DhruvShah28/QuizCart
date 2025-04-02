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

        /// <summary>
        /// Redirects the base route to the list of ingredients.
        /// </summary>
        /// <returns>Redirect to List view.</returns>

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays the list of all ingredients.
        /// </summary>
        /// <returns>View with list of ingredients.</returns>
        /// <example>GET: IngredientsPage/List</example>


        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var ingredients = await _ingredientService.ListIngredients();
            return View(ingredients);
        }

        /// <summary>
        /// Displays the details of a specific ingredient by ID.
        /// </summary>
        /// <param name="id">ID of the ingredient.</param>
        /// <returns>View with ingredient details or error view if not found.</returns>
        /// <example>GET: IngredientsPage/Details/5</example>


        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);
            if (ingredient == null)
                return View("Error", new ErrorViewModel { Errors = ["Ingredient not found."] });

            return View(ingredient);
        }

        /// <summary>
        /// Displays the form to add a new ingredient.
        /// </summary>
        /// <returns>View with empty ingredient form.</returns>
        /// <example>GET: IngredientsPage/AddIngredient</example>


        [HttpGet("AddIngredient")]
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Processes the form submission to add a new ingredient.
        /// </summary>
        /// <param name="dto">AddIngredientDto containing the ingredient data.</param>
        /// <returns>Redirect to list if successful, else redisplay form or error view.</returns>
        /// <example>POST: IngredientsPage/AddIngredient</example>


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

        /// <summary>
        /// Displays the form to edit an existing ingredient.
        /// </summary>
        /// <param name="id">ID of the ingredient to edit.</param>
        /// <returns>View with pre-filled data or error view if not found.</returns>
        /// <example>GET: IngredientsPage/EditIngredient/3</example>


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

        /// <summary>
        /// Submits the form to update an existing ingredient.
        /// </summary>
        /// <param name="id">ID of the ingredient.</param>
        /// <param name="dto">Updated ingredient information.</param>
        /// <returns>Redirect to details if successful, else error view.</returns>
        /// <example>POST: IngredientsPage/EditIngredient/3</example>


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

        /// <summary>
        /// Displays the confirmation page before deleting an ingredient.
        /// </summary>
        /// <param name="id">ID of the ingredient to delete.</param>
        /// <returns>Confirmation view or error if not found.</returns>
        /// <example>GET: IngredientsPage/DeleteIngredient/4</example>


        [HttpGet("DeleteIngredient/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);
            if (ingredient == null)
                return View("Error", new ErrorViewModel { Errors = ["Ingredient not found."] });

            return View(ingredient);
        }

        /// <summary>
        /// Deletes the ingredient after confirmation.
        /// </summary>
        /// <param name="id">ID of the ingredient to delete.</param>
        /// <returns>Redirects to list or displays error.</returns>
        /// <example>POST: IngredientsPage/DeleteIngredient/4</example>


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
