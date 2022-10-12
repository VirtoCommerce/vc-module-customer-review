using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CustomerReviews.Data.Migrations
{
    public partial class EntityIdInReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestReview_StoreId_ProductId_UserId",
                table: "RequestReview");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_StoreId_ProductId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_ProductId_ReviewStatus",
                table: "CustomerReview");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_ProductId_UserId",
                table: "CustomerReview");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "RequestReview",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Ratings",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "CustomerReview",
                newName: "EntityId");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "RequestReview",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "Product");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Ratings",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "Product");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "CustomerReview",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReview_StoreId_EntityId_EntityType_UserId",
                table: "RequestReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_StoreId_EntityId_EntityType",
                table: "Ratings",
                columns: new[] { "StoreId", "EntityId", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_ReviewStatus",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "ReviewStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestReview_StoreId_EntityId_EntityType_UserId",
                table: "RequestReview");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_StoreId_EntityId_EntityType",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_ReviewStatus",
                table: "CustomerReview");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "RequestReview");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "CustomerReview");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "RequestReview",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Ratings",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "CustomerReview",
                newName: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReview_StoreId_ProductId_UserId",
                table: "RequestReview",
                columns: new[] { "StoreId", "ProductId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_StoreId_ProductId",
                table: "Ratings",
                columns: new[] { "StoreId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_ProductId_ReviewStatus",
                table: "CustomerReview",
                columns: new[] { "StoreId", "ProductId", "ReviewStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_ProductId_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "ProductId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }
    }
}
