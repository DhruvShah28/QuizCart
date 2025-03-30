using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class memberxsubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberSubject",
                columns: table => new
                {
                    MembersMemberId = table.Column<int>(type: "int", nullable: false),
                    SubjectsSubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSubject", x => new { x.MembersMemberId, x.SubjectsSubjectId });
                    table.ForeignKey(
                        name: "FK_MemberSubject_Members_MembersMemberId",
                        column: x => x.MembersMemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberSubject_Subjects_SubjectsSubjectId",
                        column: x => x.SubjectsSubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberSubject_SubjectsSubjectId",
                table: "MemberSubject",
                column: "SubjectsSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberSubject");
        }
    }
}
