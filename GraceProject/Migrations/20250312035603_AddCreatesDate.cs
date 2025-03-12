using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraceProject.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatesDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserQuizzes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()"); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserQuizzes");
        }
    }
}
