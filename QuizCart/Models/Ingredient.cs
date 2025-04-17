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

        public string? ImagePath { get; set; }


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

        public List<BrainFoodDto> AssessmentsUsedIn { get; set; } = new();

        public string? ImagePath { get; set; }

    }


    public class UpdateIngredientDto
{
    public int IngredientId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Benefits { get; set; } = string.Empty;

    public float UnitPrice { get; set; }

    public IFormFile? ImageFile { get; set; } 
    }

public class AddIngredientDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Benefits { get; set; } = string.Empty;

    public float UnitPrice { get; set; }

    public IFormFile? ImageFile { get; set; }
    }




}