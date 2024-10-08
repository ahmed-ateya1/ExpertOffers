using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class relationBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Companies_ClientID",
                table: "Favorites");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_CompanyID",
                table: "Favorites",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Companies_CompanyID",
                table: "Favorites",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Companies_CompanyID",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_CompanyID",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Companies_ClientID",
                table: "Favorites",
                column: "ClientID",
                principalTable: "Companies",
                principalColumn: "CompanyID");
        }
    }
}
