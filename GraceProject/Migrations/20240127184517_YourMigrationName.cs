using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraceProject.Migrations
{
    /// <inheritdoc />
    public partial class YourMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    ZIPCode = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Address_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            /*Module*/
            migrationBuilder.CreateTable(
            name: "Module",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                ModuleName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                SavedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                ParentModuleId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Modules", x => x.Id);
                table.ForeignKey(
                    name: "FK_Modules_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);

                table.ForeignKey(
                    name: "FK_Modules_Modules_ParentModuleId",
                    column: x => x.ParentModuleId,
                    principalTable: "Module",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });




            /*Slide*/
            migrationBuilder.CreateTable(
            name: "Slide",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SlideSectionsType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NumSlideSections = table.Column<int>(type: "int", nullable: false),
                ModuleId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                Title = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                DisplayTitle = table.Column<bool>(type: "bit", nullable: false),
                SavedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                ShortTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DisplayThumbnailSlide = table.Column<bool>(type: "bit", nullable: false),
                ThumbIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ForwardButton = table.Column<bool>(type: "bit", nullable: false),
                BackwardButton = table.Column<bool>(type: "bit", nullable: false),
                SlideOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Slides", x => x.Id);
                table.ForeignKey(
                    name: "FK_Slides_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);

                table.ForeignKey(
                    name: "FK_Slides_Modules_ModuleId",
                    column: x => x.ModuleId,
                    principalTable: "Module",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });



            migrationBuilder.CreateTable(
            name: "SlideSection",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SlideId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                Title = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SavedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                ControllerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ControllerParameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TextToVoice = table.Column<bool>(type: "bit", nullable: false),
                SlideSectionOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SlideSections", x => x.Id);
                table.ForeignKey(
                    name: "FK_SlideSections_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);

                table.ForeignKey(
                    name: "FK_SlideSections_Slides_SlideId",
                    column: x => x.SlideId,
                    principalTable: "Slide",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
               name: "IX_Modules_ParentModuleId",
               table: "Module",
               column: "ParentModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_UserId",
                table: "Module",
                column: "UserId");

            migrationBuilder.CreateIndex(
           name: "IX_Slides_ModuleId",
           table: "Slide",
           column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Slides_UserId",
                table: "Slide",
                column: "UserId");


            migrationBuilder.CreateIndex(
           name: "IX_SlideSections_SlideId",
           table: "SlideSection",
           column: "SlideId");

            migrationBuilder.CreateIndex(
                name: "IX_SlideSections_UserId",
                table: "SlideSection",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "Slide");

            migrationBuilder.DropTable(
                name: "SlideSection");

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ZIPCode = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId",
                unique: true);
        }
    }
}
