using System;

namespace VirtoCommerce.CustomerReviews.Data.Models
{
    public class ReviewRatingCalculateDto
    {
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
