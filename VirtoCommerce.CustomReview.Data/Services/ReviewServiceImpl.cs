using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheManager.Core.Logging;
using Common.Logging;
using VirtoCommerce.CustomReview.Data.Model;
using VirtoCommerce.CustomReview.Data.Repository;

namespace VirtoCommerce.CustomReview.Data.Services
{
    public class ReviewServiceImpl : IReviewServiceImpl
    {
        private readonly IReviewRepository mReviewRepository;
        private readonly ILog mLogger;

        public ReviewServiceImpl(IReviewRepository reviewRepository, ILog logger)
        {
            mReviewRepository = reviewRepository;
            mLogger = logger;
        }

        public Review GetReviewById => throw new NotImplementedException();

        public IQueryable<Review> GetReviewsByProductId => throw new NotImplementedException();

        public decimal GetRateByProductId => throw new NotImplementedException();

        public void ApprovedReview(string[] ids)
        {
            throw new NotImplementedException();
        }

        public void DeleteReviews(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Review SaveReview(Review review)
        {
            throw new NotImplementedException();
        }
    }
}
