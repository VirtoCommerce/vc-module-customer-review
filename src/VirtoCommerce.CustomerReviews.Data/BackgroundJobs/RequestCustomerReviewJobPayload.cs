namespace VirtoCommerce.CustomerReviews.Data.BackgroundJobs
{
    /// <summary>
    /// Payload of the recurring job that emails customers a request to review what they bought. Carries no data: each
    /// occurrence processes everything that is due. Extend it via <c>AbstractTypeFactory</c> to pass parameters to an
    /// overridden handler.
    /// </summary>
    public class RequestCustomerReviewJobPayload
    {
    }
}
