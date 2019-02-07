namespace VirtoCommerce.CustomerReviews.Core.RatingCalculators
{
    public interface IRatingCalculator
    {
        string Name { get; }
        decimal Calculate(int[] ratings);
    }
}
