using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizCart.Models
{
    public enum Difficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }
    public class Assessment
    {
        [Key]
        public int AssessmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateOnly DateOfAssessment { get; set; }

        public Difficulty DifficultyLevel { get; set; }

        // one assessment belongs to one subject
        [ForeignKey("Subjects")]
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }


        // one assessment needs many brainfoods
        public ICollection<BrainFood>? BrainFoods { get; set; }

    }


    public class AssessmentDto
    {
        [Key]
        public int AssessmentId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly DateOfAssessment { get; set; }

        [Required]
        public Difficulty DifficultyLevel { get; set; }

        public string SubjectName { get; set; }

        public List<string> MemberNames { get; set; }

        public List<BrainFoodDto> BrainFoods { get; set; } = new();


    }
    public class UpdateAssessmentDto
    {
        public int AssessmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateOnly DateOfAssessment { get; set; }

        [Required]
        public Difficulty DifficultyLevel { get; set; }
    }

    public class AddAssessmentDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateOnly DateOfAssessment { get; set; }

        [Required]
        public Difficulty DifficultyLevel { get; set; }

        [Required]
        public int SubjectId { get; set; }

    }





}