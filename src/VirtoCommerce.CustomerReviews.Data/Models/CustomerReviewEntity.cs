using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CustomerReviews.Data.Models
{
    public class CustomerReviewEntity : AuditableEntity, IDataEntity<CustomerReviewEntity, CustomerReview>
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
        public string EntityId { get; set; }

        [Required]
        [StringLength(128)]
        public string EntityType { get; set; }

        [StringLength(1024)]
        public string EntityName { get; set; }

        [Required]
        [StringLength(128)]
        public string StoreId { get; set; }

        [Required]
        public int ReviewStatus { get; set; }

        #region Navigation Properties
        public virtual ObservableCollection<CustomerReviewImageEntity> Images { get; set; } = new NullCollection<CustomerReviewImageEntity>();
        #endregion

        public virtual CustomerReview ToModel(CustomerReview model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.Id = Id;
            model.CreatedBy = CreatedBy;
            model.CreatedDate = CreatedDate;
            model.ModifiedBy = ModifiedBy;
            model.ModifiedDate = ModifiedDate;

            model.UserId = UserId;
            model.UserName = UserName;

            model.Review = Review;
            model.ReviewStatus = (CustomerReviewStatus)ReviewStatus;
            model.Rating = Rating;
            model.Title = Title;

            model.EntityId = EntityId;
            model.EntityType = EntityType;
            model.EntityName = EntityName;
            model.StoreId = StoreId;

            model.Images = Images.OrderBy(x => x.SortOrder).Select(x => x.ToModel(AbstractTypeFactory<CustomerReviewImage>.TryCreateInstance())).ToList();

            return model;
        }

        public virtual CustomerReviewEntity FromModel(CustomerReview model, PrimaryKeyResolvingMap pkMap)
        {
            ArgumentNullException.ThrowIfNull(model);

            pkMap.AddPair(model, this);

            Id = model.Id;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            ModifiedBy = model.ModifiedBy;
            ModifiedDate = model.ModifiedDate;

            UserId = model.UserId;
            UserName = model.UserName;

            Title = model.Title;
            Review = model.Review;
            ReviewStatus = (byte)model.ReviewStatus;
            Rating = model.Rating;

            EntityId = model.EntityId;
            EntityType = model.EntityType;
            EntityName = model.EntityName;
            StoreId = model.StoreId;

            if (model.Images != null)
            {
                Images = new ObservableCollection<CustomerReviewImageEntity>(model.Images.Where(x => !x.IsInherited).Select(x => AbstractTypeFactory<CustomerReviewImageEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            return this;
        }

        public virtual void Patch(CustomerReviewEntity target)
        {
            ArgumentNullException.ThrowIfNull(target);

            target.UserId = UserId;
            target.UserName = UserName;

            target.Title = Title;
            target.Review = Review;
            target.Rating = Rating;
            target.ReviewStatus = ReviewStatus;

            target.EntityId = EntityId;
            target.EntityType = EntityType;
            target.EntityName = EntityName;
            target.StoreId = StoreId;

            if (!Images.IsNullCollection())
            {
                Images.Patch(target.Images, (sourceImage, targetImage) => sourceImage.Patch(targetImage));
            }
        }
    }
}
