﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuizCart.Models.ViewModels
{
    public class AddPurchaseViewModel
    {
        [Required]
        public DateOnly DatePurchased { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public int AssessmentId { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        // Dropdown sources
        public List<SelectListItem> Members { get; set; } = new();
        public List<SelectListItem> Assessments { get; set; } = new();
        public List<SelectListItem> Ingredients { get; set; } = new();
    }
}
