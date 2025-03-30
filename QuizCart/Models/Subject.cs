using System.ComponentModel.DataAnnotations;

namespace QuizCart.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // one subject can be prepared by many members
        public ICollection<Member>? Members { get; set; }

        // one subject has many assessments
        public ICollection<Assessment>? Assessments { get; set; }

    }


    public class SubjectDto
    {
        [Key]
        public int SubjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int TotalAssessments { get; set; }

        public int TotalMembers { get; set; }

    }

    public class UpdateSubjectDto
    {
        public int SubjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    public class AddSubjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }


    public class LinkSubjectDto
    {
        public int MemberId { get; set; }
        public int SubjectId { get; set; }
    }




}