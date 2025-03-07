using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraceProject.Migrations
{
    /// <inheritdoc />
    public partial class StudentReadTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Module_Course_CourseID",
                table: "Module");

            migrationBuilder.RenameColumn(
                name: "CourseID",
                table: "Module",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Module_CourseID",
                table: "Module",
                newName: "IX_Module_CourseId");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "Module",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SlideReadTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlideId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlideReadTracking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlideReadTracking_Slide_SlideId",
                        column: x => x.SlideId,
                        principalTable: "Slide",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SlideReadTracking_SlideId",
                table: "SlideReadTracking",
                column: "SlideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Module_Course_CourseId",
                table: "Module",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Module_Course_CourseId",
                table: "Module");

            migrationBuilder.DropTable(
                name: "SlideReadTracking");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Module",
                newName: "CourseID");

            migrationBuilder.RenameIndex(
                name: "IX_Module_CourseId",
                table: "Module",
                newName: "IX_Module_CourseID");

            migrationBuilder.AlterColumn<string>(
                name: "CourseID",
                table: "Module",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Module_Course_CourseID",
                table: "Module",
                column: "CourseID",
                principalTable: "Course",
                principalColumn: "CourseID");
        }
    }
}
