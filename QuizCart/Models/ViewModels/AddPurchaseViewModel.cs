using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuizCart.Models.ViewModels
{
    public class AddPurchaseViewModel
    {
        public int MemberId { get; set; }
        public DateOnly DatePurchased { get; set; }

        public int IngredientId { get; set; }
        public int Quantity { get; set; }

        public int AssessmentId { get; set; } 

        public List<SelectListItem> Members { get; set; }
        public List<SelectListItem> Ingredients { get; set; }
        public List<SelectListItem> Assessments { get; set; } 

    }
}
