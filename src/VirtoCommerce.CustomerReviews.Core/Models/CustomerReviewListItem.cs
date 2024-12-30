using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReviewListItem
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }

        [Obsolete("Use EntityName instead")]
        public string ProductName { get { return EntityName; } }

        public string ReviewStatus { get; set; }
        public byte ReviewStatusId { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<CustomerReviewListItemImage> Images { get; set; }

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
            EntityId = review.EntityId;
            EntityType = review.EntityType;
            EntityName = review.EntityName;
            Images = review.Images?.OrderBy(x => x.SortOrder).Select(x => new CustomerReviewListItemImage
            {
                Id = x.Id,
                Url = x.Url,
                RelativeUrl = x.RelativeUrl,
                Description = x.Description,
                Name = x.Name
            }).ToList();
        }
    }
}
