using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CustomerReviews.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class FixOfFieldsRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview");

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

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                table: "CustomerReview",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview");

            migrationBuilder.AlterColumn<string>(
                name: "StoreId",
                table: "CustomerReview",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                table: "CustomerReview",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_StoreId_EntityId_EntityType_UserId",
                table: "CustomerReview",
                columns: new[] { "StoreId", "EntityId", "EntityType", "UserId" },
                unique: true,
                filter: "[StoreId] IS NOT NULL AND [UserId] IS NOT NULL");
        }
    }
}
