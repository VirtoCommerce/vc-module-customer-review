using System;
using System.Linq;
using VirtoCommerce.CustomerReviews.Core.Services;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    /// <summary>
    /// Calculate rating by Edvin Wilson formula
    /// </summary>
    /// <seealso cref="https://habr.com/company/darudar/blog/143188/"/>
    public class WilsonRatingCalculator : IRatingCalculator
    {
        private const int MinRating = 0;
        private const int MaxRating = 5;
        private const int RatingInterval = MaxRating - MinRating;

        /// <summary>
        /// confidence interval
        /// 1.0 = 85%, 1.6 = 95%
        /// </summary>
        private const double Z = 1.64485;

        /// <summary>
        /// squared confidence interval
        /// </summary>
        private const double Z2 = Z * Z;

        public string Name => "Wilson";

        public decimal Calculate(int[] ratings)
        {
            if (ratings.Length == 0) return 0;
            var ratingSum = ratings.Sum();
            var ratingCount = ratings.Length;
            var ratingCount2 = ratingCount * 2;
            var ratingCount4 = ratingCount * 4;


            var phat = (double)(ratingSum - ratingCount * MinRating) / RatingInterval / ratingCount;
            var numerator = phat + Z2 / ratingCount2 - Z * Math.Sqrt((phat * (1 - phat) + Z2 / ratingCount4) / ratingCount);
            var denominator = 1 + Z2 / ratingCount;
            var reducedInterval = numerator / denominator;
            return (decimal)(reducedInterval * RatingInterval + MinRating);
        }
    }
}
