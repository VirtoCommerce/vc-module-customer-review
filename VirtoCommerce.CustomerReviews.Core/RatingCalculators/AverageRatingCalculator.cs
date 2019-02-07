using System.Linq;
using VirtoCommerce.CustomerReviews.Core.Services;

namespace VirtoCommerce.CustomerReviews.Core.RatingCalculators
{
    /// <summary>
    /// Calculate rating by geting average value 
    /// </summary>
    public class AverageRatingCalculator : IRatingCalculator
    {
        public string Name => "Average";

        public decimal Calculate(int[] ratings)
        {
            if (ratings.Length == 0) return 0;
            return (decimal)ratings.Sum() / ratings.Length;
        }
    }
}
