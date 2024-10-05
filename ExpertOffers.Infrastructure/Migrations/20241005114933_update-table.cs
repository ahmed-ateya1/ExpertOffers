using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferURL",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "TotalUsed",
                table: "Offers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OfferURL",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TotalUsed",
                table: "Offers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
