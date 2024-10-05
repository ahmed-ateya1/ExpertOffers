using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateOffersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferDescription",
                table: "Offers");

            migrationBuilder.AddColumn<long>(
                name: "TotalUsed",
                table: "Offers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalUsed",
                table: "Offers");

            migrationBuilder.AddColumn<string>(
                name: "OfferDescription",
                table: "Offers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
