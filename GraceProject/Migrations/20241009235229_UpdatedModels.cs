using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraceProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_SchoolInfo_SchoolInfoSchoolID",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Educator_SchoolInfo_SchoolInfoSchoolID",
                table: "Educator");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_SchoolInfo_SchoolInfoSchoolID",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_SchoolInfoSchoolID",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Educator_SchoolInfoSchoolID",
                table: "Educator");

            migrationBuilder.DropIndex(
                name: "IX_Course_SchoolInfoSchoolID",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "SchoolInfoSchoolID",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "SchoolInfoSchoolID",
                table: "Educator");

            migrationBuilder.DropColumn(
                name: "SchoolInfoSchoolID",
                table: "Course");

            migrationBuilder.CreateIndex(
                name: "IX_Student_SchoolID",
                table: "Student",
                column: "SchoolID");

            migrationBuilder.CreateIndex(
                name: "IX_Educator_SchoolID",
                table: "Educator",
                column: "SchoolID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_SchoolID",
                table: "Course",
                column: "SchoolID");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_SchoolInfo_SchoolID",
                table: "Course",
                column: "SchoolID",
                principalTable: "SchoolInfo",
                principalColumn: "SchoolID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Educator_SchoolInfo_SchoolID",
                table: "Educator",
                column: "SchoolID",
                principalTable: "SchoolInfo",
                principalColumn: "SchoolID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_SchoolInfo_SchoolID",
                table: "Student",
                column: "SchoolID",
                principalTable: "SchoolInfo",
                principalColumn: "SchoolID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_SchoolInfo_SchoolID",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Educator_SchoolInfo_SchoolID",
                table: "Educator");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_SchoolInfo_SchoolID",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_SchoolID",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Educator_SchoolID",
                table: "Educator");

            migrationBuilder.DropIndex(
                name: "IX_Course_SchoolID",
                table: "Course");

            migrationBuilder.AddColumn<int>(
                name: "SchoolInfoSchoolID",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SchoolInfoSchoolID",
                table: "Educator",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SchoolInfoSchoolID",
                table: "Course",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_SchoolInfoSchoolID",
                table: "Student",
                column: "SchoolInfoSchoolID");

            migrationBuilder.CreateIndex(
                name: "IX_Educator_SchoolInfoSchoolID",
                table: "Educator",
                column: "SchoolInfoSchoolID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_SchoolInfoSchoolID",
                table: "Course",
                column: "SchoolInfoSchoolID");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_SchoolInfo_SchoolInfoSchoolID",
                table: "Course",
                column: "SchoolInfoSchoolID",
                principalTable: "SchoolInfo",
                principalColumn: "SchoolID");

            migrationBuilder.AddForeignKey(
                name: "FK_Educator_SchoolInfo_SchoolInfoSchoolID",
                table: "Educator",
                column: "SchoolInfoSchoolID",
                principalTable: "SchoolInfo",
                principalColumn: "SchoolID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_SchoolInfo_SchoolInfoSchoolID",
                table: "Student",
                column: "SchoolInfoSchoolID",
                principalTable: "SchoolInfo",
                principalColumn: "SchoolID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
