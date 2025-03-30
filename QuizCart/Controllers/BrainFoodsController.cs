using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrainFoodsController : ControllerBase
    {
        private readonly IBrainFoodService _brainFoodService;

        public BrainFoodsController(IBrainFoodService brainFoodService)
        {
            _brainFoodService = brainFoodService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<BrainFoodDto>>> ListBrainFoods()
        {
            var result = await _brainFoodService.ListBrainFoods();
            return Ok(result);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<BrainFoodDto>> FindBrainFood(int id)
        {
            var brainFood = await _brainFoodService.FindBrainFood(id);
            return brainFood == null
                ? NotFound($"BrainFood with ID {id} not found.")
                : Ok(brainFood);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddBrainFood(AddBrainFoodDto dto)
        {
            var response = await _brainFoodService.AddBrainFood(dto);

            return response.Status == ServiceResponse.ServiceStatus.Error
                ? StatusCode(500, new { error = "Error adding brain food." })
                : CreatedAtAction(nameof(FindBrainFood), new { id = response.CreatedId }, new
                {
                    message = $"BrainFood added successfully with ID {response.CreatedId}",
                    brainFoodId = response.CreatedId
                });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateBrainFood(int id, UpdateBrainFoodDto dto)
        {
            if (id != dto.BrainFoodId)
                return BadRequest(new { message = "BrainFood ID mismatch." });

            var response = await _brainFoodService.UpdateBrainFood(id, dto);

            return response.Status switch
            {
                ServiceResponse.ServiceStatus.NotFound => NotFound(new { error = "BrainFood not found." }),
                ServiceResponse.ServiceStatus.Error => StatusCode(500, new { error = "Error updating brain food." }),
                _ => Ok(new { message = $"BrainFood with ID {id} updated successfully." })
            };
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteBrainFood(int id)
        {
            var response = await _brainFoodService.DeleteBrainFood(id);

            return response.Status switch
            {
                ServiceResponse.ServiceStatus.NotFound => NotFound(new { error = "BrainFood not found." }),
                ServiceResponse.ServiceStatus.Error => StatusCode(500, new { error = "Error deleting brain food." }),
                _ => Ok(new { message = $"BrainFood with ID {id} deleted successfully." })
            };
        }
    }
}
