using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CustomerReviewQueryBuilder : SearchQueryBuilder<CustomerReviewsQuery, CustomerReviewSearchResult, CustomerReview, CustomerReviewType>
{
    protected override string Name => "customerReviews";

    public CustomerReviewQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }
}
