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

        /// <summary>
        /// Retrieves a list of all ingredients with related usage in assessments.
        /// </summary>
        /// <returns>HTTP 200 OK with list of IngredientDto.</returns>
        /// <example>
        /// GET: api/Ingredients/List -> [
        ///{
        ///    "ingredientId": 1,
        ///    "name": "Spinach",
        ///    "benefits": "Rich in iron and vitamins",
        ///    "unitPrice": 0.99,
        ///    "totalAssessments": 2,
        ///    "assessmentsUsedIn": [
        ///      {
        ///        "brainFoodId": 1,
        ///        "ingredientId": 1,
        ///        "assessmentId": 3,
        ///        "quantity": 2,
        ///        "assessmentName": "Biology Quiz",
        ///        "ingredientName": "Spinach",
        ///        "benefits": "Rich in iron and vitamins",
        ///        "unitPrice": 0.99,
        ///        "purchases": [
        ///          {
        ///            "memberName": "Alice",
        ///            "datePurchased": "2025-04-02"
        ///          }
        ///        ]
        ///      }
        ///    ]
        ///  },
        ///  {
        ///    "ingredientId": 2,
        ///    "name": "Blueberry",
        ///    "benefits": "Boosts memory",
        ///    "unitPrice": 1.50,
        ///    "totalAssessments": 1,
        ///    "assessmentsUsedIn": []
        ///  }
        ///]    
        /// </example>

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> ListIngredients()
        {
            var result = await _ingredientService.ListIngredients();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves an ingredient by its ID.
        /// </summary>
        /// <param name="id">The ID of the ingredient.</param>
        /// <returns>HTTP 200 OK with IngredientDto or 404 if not found.</returns>
        /// <example>
        /// GET: api/Ingredients/Find/1 -> {
        ///  "ingredientId": 1,
        ///  "name": "Spinach",
        ///  "benefits": "Rich in iron and vitamins",
        ///  "unitPrice": 0.99,
        ///  "totalAssessments": 2,
        ///  "assessmentsUsedIn": [
        ///    {
        ///      "brainFoodId": 1,
        ///      "ingredientId": 1,
        ///      "assessmentId": 3,
        ///      "quantity": 2,
        ///      "assessmentName": "Biology Quiz",
        ///      "ingredientName": "Spinach",
        ///      "benefits": "Rich in iron and vitamins",
        ///      "unitPrice": 0.99,
        ///      "purchases": [
        ///        {
        ///          "memberName": "Alice",
        ///          "datePurchased": "2025-04-02"
        ///        },
        ///        {
        ///          "memberName": "Bob",
        ///          "datePurchased": "2025-04-04"
        ///        }
        ///      ]
        ///    }
        ///  ]
        ///}

        /// </example>



        [HttpGet("Find/{id}")]
        public async Task<ActionResult<IngredientDto>> FindIngredient(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);

            if (ingredient == null)
                return NotFound($"Ingredient with ID {id} not found.");

            return Ok(ingredient);
        }

        /// <summary>
        /// Adds a new ingredient.
        /// </summary>
        /// <param name="dto">The ingredient data to add.</param>
        /// <returns>HTTP 201 Created with new ingredient ID or 500 if error occurs.</returns>
        /// <example>
        /// POST: api/Ingredients/Add
        /// Request Body:
        /// {
        ///   "name": "Blueberry",
        ///   "benefits": "Rich in antioxidants",
        ///   "unitPrice": 1.99
        /// }
        /// </example>


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

        /// <summary>
        /// Updates an existing ingredient.
        /// </summary>
        /// <param name="id">ID of the ingredient to update.</param>
        /// <param name="dto">Updated ingredient data.</param>
        /// <returns>
        /// HTTP 200 OK if successful, 400 if ID mismatch, 404 or 500 otherwise.
        /// </returns>
        /// <example>
        /// PUT: api/Ingredients/Update/1
        /// Request Body:
        /// {
        ///   "ingredientId": 1,
        ///   "name": "Almond",
        ///   "benefits": "Improves memory",
        ///   "unitPrice": 2.50
        /// }
        /// </example>


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

        /// <summary>
        /// Deletes an ingredient by ID.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete.</param>
        /// <returns>HTTP 200 OK if deleted, 404 or 500 otherwise.</returns>
        /// <example>
        /// DELETE: api/Ingredients/Delete/1
        /// </example>


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
