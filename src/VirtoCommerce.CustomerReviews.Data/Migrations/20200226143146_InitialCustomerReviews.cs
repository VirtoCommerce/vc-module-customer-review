using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CustomerReviews.Data.Migrations
{
    public partial class InitialCustomerReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerReview",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Review = table.Column<string>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: true),
                    UserName = table.Column<string>(maxLength: 128, nullable: true),
                    ProductId = table.Column<string>(maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(maxLength: 128, nullable: false),
                    ReviewStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ProductId = table.Column<string>(maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_ProductId_ReviewStatus",
                table: "CustomerReview",
                columns: new[] { "StoreId", "ProductId", "ReviewStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_StoreId_ProductId",
                table: "Ratings",
                columns: new[] { "StoreId", "ProductId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReview");

            migrationBuilder.DropTable(
                name: "Ratings");
        }
    }
}
