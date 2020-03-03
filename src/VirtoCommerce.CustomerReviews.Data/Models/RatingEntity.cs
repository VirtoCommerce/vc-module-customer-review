using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Data.Models
{

    public class RatingEntity : Entity
    {
        [StringLength(128)]
        [Required]
        public string ProductId { get; set; }

        [StringLength(128)]
        [Required]
        public string StoreId { get; set; }

        public decimal Value { get; set; }

        public virtual void Patch(RatingEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.StoreId = StoreId;
            target.ProductId = ProductId;
            target.Value = Value;
        }
    }


}
