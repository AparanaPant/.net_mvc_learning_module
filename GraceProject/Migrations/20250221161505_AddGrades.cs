using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraceProject.Migrations
{
    /// <inheritdoc />
    public partial class AddGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Grade",
                columns: table => new
                {
                    GradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grade", x => x.GradeId);
                });

            migrationBuilder.CreateTable(
                name: "EducatorGrade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EducatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorGrade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducatorGrade_AspNetUsers_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducatorGrade_Grade_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grade",
                        principalColumn: "GradeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GradeId",
                table: "AspNetUsers",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorGrade_EducatorId",
                table: "EducatorGrade",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorGrade_GradeId",
                table: "EducatorGrade",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Grade_GradeId",
                table: "AspNetUsers",
                column: "GradeId",
                principalTable: "Grade",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Grade_GradeId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EducatorGrade");

            migrationBuilder.DropTable(
                name: "Grade");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GradeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "AspNetUsers");
        }
    }
}
