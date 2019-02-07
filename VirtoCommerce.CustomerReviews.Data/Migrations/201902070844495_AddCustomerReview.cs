namespace VirtoCommerce.CustomerReviews.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerReview : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerReview",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(),
                        Review = c.String(nullable: false),
                        Rating = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        UserName = c.String(maxLength: 128),
                        ProductId = c.String(nullable: false, maxLength: 128),
                        StoreId = c.String(nullable: false, maxLength: 128),
                        ReviewStatus = c.Byte(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.StoreId, t.ProductId, t.ReviewStatus });
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProductId = c.String(nullable: false, maxLength: 128),
                        StoreId = c.String(nullable: false, maxLength: 128),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.StoreId, t.ProductId }, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Ratings", new[] { "StoreId", "ProductId" });
            DropIndex("dbo.CustomerReview", new[] { "StoreId", "ProductId", "ReviewStatus" });
            DropTable("dbo.Ratings");
            DropTable("dbo.CustomerReview");
        }
    }
}
