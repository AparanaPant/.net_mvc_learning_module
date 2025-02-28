using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraceProject.Migrations
{
    /// <inheritdoc />
    public partial class addstudnetregistrationdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSession_AspNetUsers_StudentID",
                table: "StudentSession");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSession_Session_SessionID",
                table: "StudentSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSession",
                table: "StudentSession");

            migrationBuilder.RenameTable(
                name: "StudentSession",
                newName: "StudentSessions");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSession_StudentID",
                table: "StudentSessions",
                newName: "IX_StudentSessions_StudentID");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSession_SessionID",
                table: "StudentSessions",
                newName: "IX_StudentSessions_SessionID");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "StudentSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSessions",
                table: "StudentSessions",
                column: "StudentSessionID");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSessions_AspNetUsers_StudentID",
                table: "StudentSessions",
                column: "StudentID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSessions_Session_SessionID",
                table: "StudentSessions",
                column: "SessionID",
                principalTable: "Session",
                principalColumn: "SessionID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSessions_AspNetUsers_StudentID",
                table: "StudentSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSessions_Session_SessionID",
                table: "StudentSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSessions",
                table: "StudentSessions");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "StudentSessions");

            migrationBuilder.RenameTable(
                name: "StudentSessions",
                newName: "StudentSession");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSessions_StudentID",
                table: "StudentSession",
                newName: "IX_StudentSession_StudentID");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSessions_SessionID",
                table: "StudentSession",
                newName: "IX_StudentSession_SessionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSession",
                table: "StudentSession",
                column: "StudentSessionID");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSession_AspNetUsers_StudentID",
                table: "StudentSession",
                column: "StudentID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSession_Session_SessionID",
                table: "StudentSession",
                column: "SessionID",
                principalTable: "Session",
                principalColumn: "SessionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
