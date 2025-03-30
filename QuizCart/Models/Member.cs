using System.ComponentModel.DataAnnotations;

namespace QuizCart.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // one member can create many subjects
        public ICollection<Subject>? Subjects { get; set; }

        // one member can have many purchases
        public ICollection<Purchase>? Purchases { get; set; }

    }


    public class MemberDto
    {
        [Key]
        public int MemberId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public float AmountOwed { get; set; }

        public float AmountPaid { get; set; }

        public int TotalSubjects { get; set; }

        public int TotalAssessments { get; set; }

    }


    public class UpdateMemberDto
    {
        public int MemberId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class AddMemberDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }





}