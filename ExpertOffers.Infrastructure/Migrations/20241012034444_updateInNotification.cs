using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertOffers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateInNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Bulletins_NotificationID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Clients_NotificationID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Companies_NotificationID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Coupons_NotificationID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Offers_NotificationID",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BulletinId",
                table: "Notifications",
                column: "BulletinId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ClientID",
                table: "Notifications",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyID",
                table: "Notifications",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CouponId",
                table: "Notifications",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OfferId",
                table: "Notifications",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Bulletins_BulletinId",
                table: "Notifications",
                column: "BulletinId",
                principalTable: "Bulletins",
                principalColumn: "BulletinID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Clients_ClientID",
                table: "Notifications",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Companies_CompanyID",
                table: "Notifications",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Coupons_CouponId",
                table: "Notifications",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "CouponID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Offers_OfferId",
                table: "Notifications",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "OfferID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Bulletins_BulletinId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Clients_ClientID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Companies_CompanyID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Coupons_CouponId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Offers_OfferId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_BulletinId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ClientID",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CompanyID",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CouponId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_OfferId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Bulletins_NotificationID",
                table: "Notifications",
                column: "NotificationID",
                principalTable: "Bulletins",
                principalColumn: "BulletinID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Clients_NotificationID",
                table: "Notifications",
                column: "NotificationID",
                principalTable: "Clients",
                principalColumn: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Companies_NotificationID",
                table: "Notifications",
                column: "NotificationID",
                principalTable: "Companies",
                principalColumn: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Coupons_NotificationID",
                table: "Notifications",
                column: "NotificationID",
                principalTable: "Coupons",
                principalColumn: "CouponID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Offers_NotificationID",
                table: "Notifications",
                column: "NotificationID",
                principalTable: "Offers",
                principalColumn: "OfferID");
        }
    }
}
