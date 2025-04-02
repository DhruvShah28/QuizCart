using Microsoft.AspNetCore.Mvc;
using QuizCart.Interfaces;
using QuizCart.Models;

namespace QuizCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Retrieves a list of all purchases.
        /// </summary>
        /// <returns>HTTP 200 OK with list of PurchasesDto.</returns>
        /// <example>
        /// GET: api/Purchases/List
        /// Response:
        /// [
        ///   {
        ///     "purchaseId": 1,
        ///     "datePurchased": "2025-04-01",
        ///     "memberName": "Alice",
        ///     "totalAmount": 6.00,
        ///     "ingredientNames": ["Almond", "Blueberry"],
        ///     "items": [
        ///       { "ingredientName": "Almond", "quantity": 2, "unitPrice": 1.5 },
        ///       { "ingredientName": "Blueberry", "quantity": 1, "unitPrice": 3.0 }
        ///     ]
        ///   }
        /// ]
        /// </example>


        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<PurchasesDto>>> ListPurchases()
        {
            var result = await _purchaseService.ListPurchases();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific purchase by its ID.
        /// </summary>
        /// <param name="id">The ID of the purchase to retrieve.</param>
        /// <returns>HTTP 200 OK with PurchasesDto or 404 Not Found.</returns>
        /// <example>
        /// GET: api/Purchases/Find/1
        /// </example>


        [HttpGet("Find/{id}")]
        public async Task<ActionResult<PurchasesDto>> FindPurchase(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);

            return purchase == null
                ? NotFound($"Purchase with ID {id} not found.")
                : Ok(purchase);
        }

        /// <summary>
        /// Creates a new purchase.
        /// </summary>
        /// <param name="dto">The purchase data to add.</param>
        /// <returns>HTTP 201 Created with new purchase ID or 500 Internal Server Error.</returns>
        /// <example>
        /// POST: api/Purchases/Add
        /// Request Body:
        /// {
        ///   "datePurchased": "2025-04-01",
        ///   "memberId": 1,
        ///   "brainFoodIds": [1, 2]
        /// }
        /// </example>


        [HttpPost("Add")]
        public async Task<IActionResult> AddPurchase(AddPurchasesDto dto)
        {
            var response = await _purchaseService.AddPurchase(dto);

            return response.Status == ServiceResponse.ServiceStatus.Error
                ? StatusCode(500, new { error = "Error adding purchase." })
                : CreatedAtAction(nameof(FindPurchase), new { id = response.CreatedId }, new
                {
                    message = $"Purchase created successfully with ID {response.CreatedId}",
                    purchaseId = response.CreatedId
                });
        }

        /// <summary>
        /// Updates an existing purchase.
        /// </summary>
        /// <param name="id">The ID of the purchase to update.</param>
        /// <param name="dto">Updated purchase data.</param>
        /// <returns>HTTP 200 OK, 404 Not Found, or 500 Internal Server Error.</returns>
        /// <example>
        /// PUT: api/Purchases/Update/1
        /// Request Body:
        /// {
        ///   "purchaseId": 1,
        ///   "datePurchased": "2025-04-02",
        ///   "brainFoodIds": [2, 3]
        /// }
        /// </example>


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, UpdatePurchasesDto dto)
        {
            var response = await _purchaseService.UpdatePurchase(id, dto);

            return response.Status switch
            {
                ServiceResponse.ServiceStatus.NotFound => NotFound(new { error = "Purchase not found." }),
                ServiceResponse.ServiceStatus.Error => StatusCode(500, new { error = "Error updating purchase." }),
                _ => Ok(new { message = $"Purchase with ID {id} updated successfully." })
            };
        }

        /// <summary>
        /// Deletes a purchase by ID.
        /// </summary>
        /// <param name="id">The ID of the purchase to delete.</param>
        /// <returns>HTTP 200 OK, 404 Not Found, or 500 Internal Server Error.</returns>
        /// <example>
        /// DELETE: api/Purchases/Delete/1
        /// </example>


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var response = await _purchaseService.DeletePurchase(id);

            return response.Status switch
            {
                ServiceResponse.ServiceStatus.NotFound => NotFound(new { error = "Purchase not found." }),
                ServiceResponse.ServiceStatus.Error => StatusCode(500, new { error = "Error deleting purchase." }),
                _ => Ok(new { message = $"Purchase with ID {id} deleted successfully." })
            };
        }

        /// <summary>
        /// Links a brain food item to a purchase.
        /// </summary>
        /// <param name="dto">The LinkBrainFoodDto containing purchase and brain food IDs.</param>
        /// <returns>HTTP 200 OK or 404 Not Found.</returns>
        /// <example>
        /// POST: api/Purchases/LinkBrainFood
        /// {
        ///   "purchaseId": 1,
        ///   "brainFoodId": 2
        /// }
        /// </example>


        [HttpPost("LinkBrainFood")]
        public async Task<IActionResult> LinkBrainFood([FromBody] LinkBrainFoodDto dto)
        {
            var result = await _purchaseService.LinkBrainFood(dto);
            return result.Status switch
            {
                ServiceResponse.ServiceStatus.NotFound => NotFound(result.Messages),
                _ => Ok(new { message = "BrainFood linked to purchase." })
            };
        }

        /// <summary>
        /// Unlinks a brain food item from a purchase.
        /// </summary>
        /// <param name="dto">The LinkBrainFoodDto containing purchase and brain food IDs.</param>
        /// <returns>HTTP 200 OK or 404 Not Found.</returns>
        /// <example>
        /// DELETE: api/Purchases/UnlinkBrainFood
        /// {
        ///   "purchaseId": 1,
        ///   "brainFoodId": 2
        /// }
        /// </example>


        [HttpDelete("UnlinkBrainFood")]
        public async Task<IActionResult> UnlinkBrainFood([FromBody] LinkBrainFoodDto dto)
        {
            var result = await _purchaseService.UnlinkBrainFood(dto);
            return result.Status switch
            {
                ServiceResponse.ServiceStatus.NotFound => NotFound(result.Messages),
                _ => Ok(new { message = "BrainFood unlinked from purchase." })
            };
        }

        /// <summary>
        /// Retrieves all purchases made by a specific member.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>HTTP 200 OK with list of PurchasesDto.</returns>
        /// <example>
        /// GET: api/Purchases/Purchases/1
        /// </example>


        [HttpGet("Purchases/{memberId}")]
        public async Task<ActionResult<IEnumerable<PurchasesDto>>> ListPurchasesByMemberId(int memberId)
        {
            var purchases = await _purchaseService.ListPurchasesByMemberId(memberId);
            return Ok(purchases);
        }


    }
}
