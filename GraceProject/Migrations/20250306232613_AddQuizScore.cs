using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraceProject.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                table: "Quizzes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "Quizzes");
        }
    }
}
