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

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<PurchasesDto>>> ListPurchases()
        {
            var result = await _purchaseService.ListPurchases();
            return Ok(result);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<PurchasesDto>> FindPurchase(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);

            return purchase == null
                ? NotFound($"Purchase with ID {id} not found.")
                : Ok(purchase);
        }

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


        [HttpGet("Purchases/{memberId}")]
        public async Task<ActionResult<IEnumerable<PurchasesDto>>> ListPurchasesByMemberId(int memberId)
        {
            var purchases = await _purchaseService.ListPurchasesByMemberId(memberId);
            return Ok(purchases);
        }


    }
}
