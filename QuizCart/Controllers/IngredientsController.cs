using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> ListIngredients()
        {
            var result = await _ingredientService.ListIngredients();
            return Ok(result);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<IngredientDto>> FindIngredient(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);

            if (ingredient == null)
                return NotFound($"Ingredient with ID {id} not found.");

            return Ok(ingredient);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddIngredient(AddIngredientDto dto)
        {
            var response = await _ingredientService.AddIngredient(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Error adding ingredient." });

            return CreatedAtAction(nameof(FindIngredient), new { id = response.CreatedId }, new
            {
                message = $"Ingredient added successfully with ID {response.CreatedId}",
                ingredientId = response.CreatedId
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, UpdateIngredientDto dto)
        {
            if (id != dto.IngredientId)
                return BadRequest(new { message = "Ingredient ID mismatch." });

            var response = await _ingredientService.UpdateIngredient(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(new { error = "Ingredient not found." });

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Unexpected error updating ingredient." });

            return Ok(new { message = $"Ingredient with ID {id} updated successfully." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var response = await _ingredientService.DeleteIngredient(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
                return NotFound(new { error = "Ingredient not found." });

            if (response.Status == ServiceResponse.ServiceStatus.Error)
                return StatusCode(500, new { error = "Unexpected error deleting ingredient." });

            return Ok(new { message = $"Ingredient with ID {id} deleted successfully." });
        }
    }
}
