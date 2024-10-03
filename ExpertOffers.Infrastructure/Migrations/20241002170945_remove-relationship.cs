using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removerelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Companies_CompanyID",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CompanyID",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "Cities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyID",
                table: "Cities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CompanyID",
                table: "Cities",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Companies_CompanyID",
                table: "Cities",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID");
        }
    }
}
