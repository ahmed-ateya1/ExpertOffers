using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GenreBulletins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GenreID",
                table: "Bulletins",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BulletinGenres",
                columns: table => new
                {
                    GenreID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulletinGenres", x => x.GenreID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bulletins_GenreID",
                table: "Bulletins",
                column: "GenreID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bulletins_BulletinGenres_GenreID",
                table: "Bulletins",
                column: "GenreID",
                principalTable: "BulletinGenres",
                principalColumn: "GenreID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bulletins_BulletinGenres_GenreID",
                table: "Bulletins");

            migrationBuilder.DropTable(
                name: "BulletinGenres");

            migrationBuilder.DropIndex(
                name: "IX_Bulletins_GenreID",
                table: "Bulletins");

            migrationBuilder.DropColumn(
                name: "GenreID",
                table: "Bulletins");
        }
    }
}
