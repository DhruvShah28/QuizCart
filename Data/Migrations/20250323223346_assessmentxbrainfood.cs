using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class assessmentxbrainfood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssessmentId",
                table: "BrainFoods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BrainFoods_AssessmentId",
                table: "BrainFoods",
                column: "AssessmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrainFoods_Assessments_AssessmentId",
                table: "BrainFoods",
                column: "AssessmentId",
                principalTable: "Assessments",
                principalColumn: "AssessmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrainFoods_Assessments_AssessmentId",
                table: "BrainFoods");

            migrationBuilder.DropIndex(
                name: "IX_BrainFoods_AssessmentId",
                table: "BrainFoods");

            migrationBuilder.DropColumn(
                name: "AssessmentId",
                table: "BrainFoods");
        }
    }
}
