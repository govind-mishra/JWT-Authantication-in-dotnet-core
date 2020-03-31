using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiUsingCore.Data.Migrations
{
    public partial class Added_RefreshTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RToken = table.Column<string>(nullable: false),
                    JWTToken = table.Column<string>(nullable: true),
                    RTokenCreationDate = table.Column<DateTime>(nullable: false),
                    RTokenExpiryDate = table.Column<DateTime>(nullable: false),
                    isUsedToken = table.Column<bool>(nullable: false),
                    inValidated = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RToken);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
