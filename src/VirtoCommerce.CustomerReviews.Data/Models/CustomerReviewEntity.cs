using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Data.Models
{
    public class CustomerReviewEntity : AuditableEntity
    {
        public string Title { get; set; }
        [Required]
        public string Review { get; set; }
        [Required]
        public int Rating { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        public string ProductId { get; set; }

        [Required]
        [StringLength(128)]
        public string StoreId { get; set; }

        [Required]
        public int ReviewStatus { get; set; }


        public virtual CustomerReview ToModel(CustomerReview customerReview)
        {
            if (customerReview == null)
                throw new ArgumentNullException(nameof(customerReview));

            customerReview.Id = Id;
            customerReview.CreatedBy = CreatedBy;
            customerReview.CreatedDate = CreatedDate;
            customerReview.ModifiedBy = ModifiedBy;
            customerReview.ModifiedDate = ModifiedDate;

            customerReview.UserId = UserId;
            customerReview.UserName = UserName;

            customerReview.Review = Review;
            customerReview.ReviewStatus = (CustomerReviewStatus)ReviewStatus;
            customerReview.Rating = Rating;
            customerReview.Title = Title;

            customerReview.ProductId = ProductId;
            customerReview.StoreId = StoreId;

            return customerReview;
        }

        public virtual CustomerReviewEntity FromModel(CustomerReview customerReview, PrimaryKeyResolvingMap pkMap)
        {
            if (customerReview == null)
                throw new ArgumentNullException(nameof(customerReview));

            pkMap.AddPair(customerReview, this);

            Id = customerReview.Id;
            CreatedBy = customerReview.CreatedBy;
            CreatedDate = customerReview.CreatedDate;
            ModifiedBy = customerReview.ModifiedBy;
            ModifiedDate = customerReview.ModifiedDate;

            UserId = customerReview.UserId;
            UserName = customerReview.UserName;

            Title = customerReview.Title;
            Review = customerReview.Review;
            ReviewStatus = (byte)customerReview.ReviewStatus;
            Rating = customerReview.Rating;

            ProductId = customerReview.ProductId;
            StoreId = customerReview.StoreId;

            return this;
        }

        public virtual void Patch(CustomerReviewEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.UserId = UserId;
            target.UserName = UserName;

            target.Title = Title;
            target.Review = Review;
            target.Rating = Rating;
            target.ReviewStatus = ReviewStatus;

            target.ProductId = ProductId;
            target.StoreId = StoreId;
        }
    }
}
