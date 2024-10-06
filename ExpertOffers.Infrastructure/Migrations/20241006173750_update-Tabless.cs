using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTabless : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponTitle",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponTitle",
                table: "Coupons");
        }
    }
}
