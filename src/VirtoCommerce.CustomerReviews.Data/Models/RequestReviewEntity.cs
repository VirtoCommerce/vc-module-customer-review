using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Data.Models
{

    public class RequestReviewEntity : Entity
    {
        [StringLength(128)]
        [Required]
        public string CustomerOrderId { get; set; }

        [StringLength(128)]
        [Required]
        public string ProductId { get; set; }

        [StringLength(128)]
        [Required]
        public string StoreId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        public DateTime? AccessDate { get; set; }

        [StringLength(128)]
        [Required]
        public string UserId { get; set; }

        [Required]
        public int ReviewsRequest { get; set; }

        public virtual void Patch(RequestReviewEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.CreatedDate = CreatedDate;
            target.ProductId = ProductId;
            target.CustomerOrderId = CustomerOrderId;
            target.AccessDate = AccessDate;
            target.UserId = UserId;
            target.ReviewsRequest = ReviewsRequest;            
        }
    }


}
