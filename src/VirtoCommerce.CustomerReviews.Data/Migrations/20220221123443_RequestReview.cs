using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CustomerReviews.Data.Migrations
{
    public partial class RequestReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestReview",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CustomerOrderId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ReviewsRequest = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestReview", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_ProductId_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "ProductId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReview_StoreId_ProductId_UserId",
                table: "RequestReview",
                columns: new[] { "StoreId", "ProductId", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestReview");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_ProductId_UserId",
                table: "CustomerReview");
        }
    }
}
