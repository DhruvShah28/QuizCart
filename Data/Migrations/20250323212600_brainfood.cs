using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class brainfood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrainFoods",
                columns: table => new
                {
                    BrainFoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrainFoods", x => x.BrainFoodId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrainFoods");
        }
    }
}
