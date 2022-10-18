using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Data.Models
{

    public class RatingEntity : Entity
    {
        [StringLength(128)]
        [Required]
        public string EntityId { get; set; }

        [Required]
        [StringLength(128)]
        public string EntityType { get; set; }

        [StringLength(128)]
        public string StoreId { get; set; }

        public decimal Value { get; set; }

        public int ReviewCount { get; set; }

        public virtual void Patch(RatingEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.StoreId = StoreId;
            target.EntityId = EntityId;
            target.EntityType = EntityType;
            target.Value = Value;
            target.ReviewCount = ReviewCount;
        }
    }
}
