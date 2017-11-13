using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CustomReview.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomReview.Data.Services
{
    public interface IReviewServiceImpl
    {
        Review GetReviewById { get; }
        IQueryable<Review> GetReviewsByProductId { get; }

        Review SaveReview(Review review);
        decimal GetRateByProductId { get; }

        void ApprovedReview(string[] ids);
        void DeleteReviews(string[] ids);
    }
}
