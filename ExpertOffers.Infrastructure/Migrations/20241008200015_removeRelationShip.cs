using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedItems_Bulletins_BulletinID",
                table: "SavedItems");

            migrationBuilder.DropIndex(
                name: "IX_SavedItems_BulletinID",
                table: "SavedItems");

            migrationBuilder.DropColumn(
                name: "BulletinID",
                table: "SavedItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BulletinID",
                table: "SavedItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedItems_BulletinID",
                table: "SavedItems",
                column: "BulletinID");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedItems_Bulletins_BulletinID",
                table: "SavedItems",
                column: "BulletinID",
                principalTable: "Bulletins",
                principalColumn: "BulletinID");
        }
    }
}
