using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CustomReview.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomReview.Data.Repository
{
    public interface IReviewRepository : IRepository
    {
        IQueryable<Review> ReviewList { get; }
        Review GetReviewById { get; }
        void DeleteReviews(string[] ids);
        double GetRateByProductId(string id);
    }
}
