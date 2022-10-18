using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CustomerReviews.Data.Migrations
{
    public partial class NullableStoreIdInReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_StoreId_EntityId_EntityType",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview");

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                table: "RequestReview",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                table: "Ratings",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                table: "CustomerReview",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_StoreId_EntityId_EntityType",
                table: "Ratings",
                columns: new[] { "StoreId", "EntityId", "EntityType" },
                unique: true,
                filter: "[StoreId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" },
                unique: true,
                filter: "[StoreId] IS NOT NULL AND [UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_StoreId_EntityId_EntityType",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview");

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                table: "RequestReview",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                table: "Ratings",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                table: "CustomerReview",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_StoreId_EntityId_EntityType",
                table: "Ratings",
                columns: new[] { "StoreId", "EntityId", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }
    }
}
