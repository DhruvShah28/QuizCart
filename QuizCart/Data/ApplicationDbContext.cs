using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizCart.Models;

namespace QuizCart.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<BrainFood> BrainFoods { get; set; }
    }
}
