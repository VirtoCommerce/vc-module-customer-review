using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CustomerReviews.Data.Models;

public class CustomerReviewImageEntity : AuditableEntity, IDataEntity<CustomerReviewImageEntity, CustomerReviewImage>
{
    [StringLength(2083)]
    [Required]
    public string Url { get; set; }

    [StringLength(1024)]
    public string Name { get; set; }

    [StringLength(5)]
    public string LanguageCode { get; set; }

    public int SortOrder { get; set; }

    [StringLength(1024)]
    public string Description { get; set; }

    [Required]
    [StringLength(128)]
    public string CustomerReviewId { get; set; }

    #region Navigation Properties

    public virtual CustomerReviewEntity CustomerReview { get; set; }

    #endregion

    public virtual CustomerReviewImage ToModel(CustomerReviewImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        image.Id = Id;
        image.CreatedBy = CreatedBy;
        image.CreatedDate = CreatedDate;
        image.ModifiedBy = ModifiedBy;
        image.ModifiedDate = ModifiedDate;

        image.LanguageCode = LanguageCode;
        image.Name = Name;
        image.SortOrder = SortOrder;
        image.Url = Url;
        image.RelativeUrl = Url;
        image.Description = Description;
        image.CustomerReviewId = CustomerReviewId;

        return image;
    }

    public virtual CustomerReviewImageEntity FromModel(CustomerReviewImage image, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(image);

        pkMap.AddPair(image, this);

        Id = image.Id;
        CreatedBy = image.CreatedBy;
        CreatedDate = image.CreatedDate;
        ModifiedBy = image.ModifiedBy;
        ModifiedDate = image.ModifiedDate;

        LanguageCode = image.LanguageCode;
        Name = image.Name;
        SortOrder = image.SortOrder;
        Description = image.Description;
        Url = !string.IsNullOrEmpty(image.RelativeUrl) ? image.RelativeUrl : image.Url;
        CustomerReviewId = image.CustomerReviewId;

        return this;
    }

    public virtual void Patch(CustomerReviewImageEntity target)
    {
        target.LanguageCode = LanguageCode;
        target.Name = Name;
        target.SortOrder = SortOrder;
        target.Url = Url;
        target.Description = Description;
    }
}
