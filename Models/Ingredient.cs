using System.ComponentModel.DataAnnotations;

namespace QuizCart.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Benefits { get; set; }

        public float UnitPrice { get; set; }

        // one ingredient can be in multiple brainfoods
        public ICollection<BrainFood>? BrainFoods { get; set; }

    }

    public class IngredientDto
    {
        [Key]
        public int IngredientId { get; set; }

        public string Name { get; set; }

        public string Benefits { get; set; }

        public float UnitPrice { get; set; }

        public int TotalAssessments { get; set; }

    }


    public class UpdateIngredientDto
    {
        [Key]
        public int IngredientId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Benefits { get; set; }

        public float UnitPrice { get; set; }

    }

    public class AddIngredientDto
    {
        public string Name { get; set; }

        public string Benefits { get; set; }

        public float UnitPrice { get; set; }

    }



}