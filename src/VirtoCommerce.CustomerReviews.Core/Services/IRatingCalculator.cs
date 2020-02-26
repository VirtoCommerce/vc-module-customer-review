namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface IRatingCalculator
    {
        string Name { get; }
        decimal Calculate(int[] ratings);
    }
}
