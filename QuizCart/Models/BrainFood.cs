using QuizCart.Data.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QuizCart.Models
{
    public class BrainFood
    {
        [Key]
        public int BrainFoodId { get; set; }

        public int Quantity { get; set; }

        // one brainfood can be purchased multiple times
        public ICollection<Purchase>? Purchases { get; set; }


        // one brainfood belongs to one assessment
        [ForeignKey("Assessments")]
        public int AssessmentId { get; set; }
        public virtual Assessment Assessment { get; set; }


        // one brainfood contains only one ingredient
        [ForeignKey("Ingredients")]
        public int IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }

    }

    public class BrainFoodDto
    {

        [Key]
        public int BrainFoodId { get; set; }

        public int Quantity { get; set; }

        public string AssessmentName {  get; set; }

        public string IngredientName { get; set; }


    }
    public class UpdateBrainFoodDto
    {
        public int BrainFoodId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [Required]
        public int AssessmentId { get; set; }
    }

    public class AddBrainFoodDto
    {
        [Required]
        public int Quantity { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [Required]
        public int AssessmentId { get; set; }
    }


    public class LinkBrainFoodDto
    {
        public int PurchaseId { get; set; }
        public int BrainFoodId { get; set; }
    }



}