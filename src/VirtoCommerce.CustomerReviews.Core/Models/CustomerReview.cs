using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReview : AuditableEntity, ICloneable
    {
        public string Title { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public string EntityName { get; set; }
        public string StoreId { get; set; }
        public CustomerReviewStatus ReviewStatus { get; set; }

        #region IHasImages members
        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>
        /// The image source URL.
        /// </value>
        public string ImgSrc
        {
            get
            {
                string result = null;
                if (Images?.Count > 0)
                {
                    result = Images.MinBy(x => x.SortOrder)?.Url;
                }
                return result;
            }
        }

        public IList<CustomerReviewImage> Images { get; set; }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            var result = (CustomerReview)MemberwiseClone();

            result.Images = Images?.Select(x => x.CloneTyped()).ToList();

            return result;
        }
        #endregion
    }
}
