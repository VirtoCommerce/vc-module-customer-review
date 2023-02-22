using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CustomerReviews.Data.PostgreSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerReview",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Review = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    UserName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EntityId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityName = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    StoreId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ReviewStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestReview",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CustomerOrderId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccessDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ReviewsRequest = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestReview", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_ReviewStatus",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "ReviewStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_StoreId_EntityId_EntityType",
                table: "Ratings",
                columns: new[] { "StoreId", "EntityId", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestReview_StoreId_EntityId_EntityType_UserId",
                table: "RequestReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReview");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "RequestReview");
        }
    }
}
