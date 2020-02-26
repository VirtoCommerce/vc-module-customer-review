using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CustomerReviews.Data.Migrations
{
    public partial class UpdateCustomerReviewsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                    IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'CustomerReviews.Data.Migrations.Configuration'))
                        BEGIN
                            INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20200226143146_InitialCustomerReviews', '3.1.0')
                        END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
