using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class ingredientxbrainfood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IngredientId",
                table: "BrainFoods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BrainFoods_IngredientId",
                table: "BrainFoods",
                column: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrainFoods_Ingredients_IngredientId",
                table: "BrainFoods",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "IngredientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrainFoods_Ingredients_IngredientId",
                table: "BrainFoods");

            migrationBuilder.DropIndex(
                name: "IX_BrainFoods_IngredientId",
                table: "BrainFoods");

            migrationBuilder.DropColumn(
                name: "IngredientId",
                table: "BrainFoods");
        }
    }
}
