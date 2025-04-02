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

        /// <summary>
        /// Retrieves a list of all brain food items.
        /// </summary>
        /// <returns>HTTP 200 OK with list of BrainFoodDto.</returns>
        /// <example>
        /// GET: api/BrainFoods/List -> [
        ///{
///    "brainFoodId": 1,
///    "ingredientId": 1,
///    "assessmentId": 2,
///    "quantity": 3,
///    "assessmentName": "Math Test",
///    "ingredientName": "Almond",
///    "benefits": "Boosts memory and concentration",
///    "unitPrice": 1.50,
///    "purchases": [
///      {
///        "memberName": "Alice",
///        "datePurchased": "2025-04-01"
///      },
///      {
///        "memberName": "Bob",
///        "datePurchased": "2025-04-02"
///      }
///    ]
///  },
///  {
///    "brainFoodId": 2,
///    "ingredientId": 2,
///    "assessmentId": 3,
///    "quantity": 5,
///    "assessmentName": "Science Quiz",
///  "ingredientName": "Blueberry",
///    "benefits": "Rich in antioxidants",
///    "unitPrice": 2.00,
///    "purchases": []
///  }
///]

        /// </example>


        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<BrainFoodDto>>> ListBrainFoods()
        {
            var result = await _brainFoodService.ListBrainFoods();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a brain food item by its ID.
        /// </summary>
        /// <param name="id">The ID of the brain food item.</param>
        /// <returns>HTTP 200 OK with BrainFoodDto or 404 if not found.</returns>
        /// <example>
        /// GET: api/BrainFoods/Find/1 -> {
        ///    "brainFoodId": 1,
        ///    "ingredientId": 1,
        ///    "assessmentId": 2,
        ///    "quantity": 3,
        ///    "assessmentName": "Math Test",
        ///    "ingredientName": "Almond",
        ///    "benefits": "Boosts memory and concentration",
        ///    "unitPrice": 1.50,
        ///    "purchases": [
        ///      {
        ///        "memberName": "Alice",
        ///        "datePurchased": "2025-04-01"
        ///      },
        ///      {
        ///        "memberName": "Bob",
        ///        "datePurchased": "2025-04-02"
        ///      }
        ///    ]
        ///  }
        /// </example>

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<BrainFoodDto>> FindBrainFood(int id)
        {
            var brainFood = await _brainFoodService.FindBrainFood(id);
            return brainFood == null
                ? NotFound($"BrainFood with ID {id} not found.")
                : Ok(brainFood);
        }

        /// <summary>
        /// Adds a new brain food item.
        /// </summary>
        /// <param name="dto">The brain food data to add.</param>
        /// <returns>HTTP 201 Created with new brain food ID or 500 if error occurs.</returns>
        /// <example>
        /// POST: api/BrainFoods/Add
        /// Request Body:
        /// {
        ///   "quantity": 3,
        ///   "ingredientId": 1,
        ///   "assessmentId": 2
        /// }
        /// </example>


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

        /// <summary>
        /// Updates an existing brain food item.
        /// </summary>
        /// <param name="id">ID of the brain food to update.</param>
        /// <param name="dto">Updated brain food data.</param>
        /// <returns>
        /// HTTP 200 OK if successful, 400 if ID mismatch, 404 or 500 otherwise.
        /// </returns>
        /// <example>
        /// PUT: api/BrainFoods/Update/1
        /// Request Body:
        /// {
        ///   "brainFoodId": 1,
        ///   "quantity": 5,
        ///   "ingredientId": 2,
        ///   "assessmentId": 3
        /// }
        /// </example>


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

        /// <summary>
        /// Deletes a brain food item by ID.
        /// </summary>
        /// <param name="id">The ID of the brain food to delete.</param>
        /// <returns>HTTP 200 OK if deleted, 404 or 500 otherwise.</returns>
        /// <example>
        /// DELETE: api/BrainFoods/Delete/1
        /// </example>


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
