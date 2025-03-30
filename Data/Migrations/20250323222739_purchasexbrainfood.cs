using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class purchasexbrainfood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrainFoodPurchase",
                columns: table => new
                {
                    BrainFoodsBrainFoodId = table.Column<int>(type: "int", nullable: false),
                    PurchasesPurchaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrainFoodPurchase", x => new { x.BrainFoodsBrainFoodId, x.PurchasesPurchaseId });
                    table.ForeignKey(
                        name: "FK_BrainFoodPurchase_BrainFoods_BrainFoodsBrainFoodId",
                        column: x => x.BrainFoodsBrainFoodId,
                        principalTable: "BrainFoods",
                        principalColumn: "BrainFoodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrainFoodPurchase_Purchases_PurchasesPurchaseId",
                        column: x => x.PurchasesPurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "PurchaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrainFoodPurchase_PurchasesPurchaseId",
                table: "BrainFoodPurchase",
                column: "PurchasesPurchaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrainFoodPurchase");
        }
    }
}
