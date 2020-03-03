using System;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReviewListItem
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string ReviewStatus { get; set; }
        public byte ReviewStatusId { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public DateTime CreatedDate { get; set; }

        public CustomerReviewListItem(CustomerReview review)
        {
            Id = review.Id;
            ReviewStatus = review.ReviewStatus.ToString();
            ReviewStatusId = (byte)review.ReviewStatus;
            Title = review.Title;
            Review = review.Review;
            Rating = review.Rating;
            UserName = review.UserName;
            CreatedDate = review.CreatedDate;
        }

    }
}