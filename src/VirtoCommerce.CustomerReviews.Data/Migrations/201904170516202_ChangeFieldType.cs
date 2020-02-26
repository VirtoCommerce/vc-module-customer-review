namespace VirtoCommerce.CustomerReviews.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFieldType : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CustomerReview", new[] { "StoreId", "ProductId", "ReviewStatus" });
            AlterColumn("dbo.CustomerReview", "ReviewStatus", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomerReview", new[] { "StoreId", "ProductId", "ReviewStatus" });
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerReview", new[] { "StoreId", "ProductId", "ReviewStatus" });
            AlterColumn("dbo.CustomerReview", "ReviewStatus", c => c.Byte(nullable: false));
            CreateIndex("dbo.CustomerReview", new[] { "StoreId", "ProductId", "ReviewStatus" });
        }
    }
}
