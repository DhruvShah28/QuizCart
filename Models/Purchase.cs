using System.ComponentModel.DataAnnotations;
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

        public int TotalAmount { get; set; }

        public List<string>? IngredientNames { get; set; }

    }


    public class UpdatePurchasesDto
    {
        [Key]
        public int PurchaseId { get; set; }

        public DateOnly DatePurchased { get; set; }

    }

    public class AddPurchasesDto
    {
        public DateOnly DatePurchased { get; set; }

    }

}