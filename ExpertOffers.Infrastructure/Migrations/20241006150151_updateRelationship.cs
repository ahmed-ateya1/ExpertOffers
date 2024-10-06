using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenreCoupons_Coupons_CouponID",
                table: "GenreCoupons");

            migrationBuilder.DropIndex(
                name: "IX_GenreCoupons_CouponID",
                table: "GenreCoupons");

            migrationBuilder.DropColumn(
                name: "CouponID",
                table: "GenreCoupons");

            migrationBuilder.AddColumn<string>(
                name: "GenreName",
                table: "GenreCoupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "GenreID",
                table: "Coupons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_GenreID",
                table: "Coupons",
                column: "GenreID");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_GenreCoupons_GenreID",
                table: "Coupons",
                column: "GenreID",
                principalTable: "GenreCoupons",
                principalColumn: "GenreID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_GenreCoupons_GenreID",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_GenreID",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "GenreName",
                table: "GenreCoupons");

            migrationBuilder.DropColumn(
                name: "GenreID",
                table: "Coupons");

            migrationBuilder.AddColumn<Guid>(
                name: "CouponID",
                table: "GenreCoupons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GenreCoupons_CouponID",
                table: "GenreCoupons",
                column: "CouponID");

            migrationBuilder.AddForeignKey(
                name: "FK_GenreCoupons_Coupons_CouponID",
                table: "GenreCoupons",
                column: "CouponID",
                principalTable: "Coupons",
                principalColumn: "CouponID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
