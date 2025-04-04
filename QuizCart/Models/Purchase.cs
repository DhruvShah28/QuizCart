﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizCart.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        public DateOnly DatePurchased { get; set; }

        // one purchase can be done by one member
        [ForeignKey("Members")]
        public int MemberId { get; set; }
        public virtual Member Member { get; set; }


        // one purchase can have many brainfood items
        public ICollection<BrainFood>? BrainFoods { get; set; }

    }



    public class PurchasesDto
    {
        [Key]
        public int PurchaseId { get; set; }

        public DateOnly DatePurchased { get; set; }

        public string MemberName { get; set; }

        public float TotalAmount { get; set; }

        public List<string>? IngredientNames { get; set; }

        public List<PurchaseItemDto> Items { get; set; } = new();

    }


    public class UpdatePurchasesDto
    {
        public int PurchaseId { get; set; }

        [Required]
        public DateOnly DatePurchased { get; set; }

        public List<int> BrainFoodIds { get; set; }
    }

    public class AddPurchasesDto
    {
        [Required]
        public DateOnly DatePurchased { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public List<int> BrainFoodIds { get; set; } = new();
    }


    public class PurchaseItemDto
    {
        public string IngredientName { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }

        public float Total => Quantity * UnitPrice;
    }

}