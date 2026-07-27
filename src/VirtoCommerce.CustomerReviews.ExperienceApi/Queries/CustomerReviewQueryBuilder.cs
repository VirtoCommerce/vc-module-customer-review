using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CustomerReviewQueryBuilder : SearchQueryBuilder<CustomerReviewsQuery, CustomerReviewSearchResult, CustomerReview, CustomerReviewType>
{
    protected override string Name => "customerReviews";

    public CustomerReviewQueryBuilder(IAuthorizationService authorizationService) : base(authorizationService)
    {
    }

    [Obsolete("Use the constructor without IMediator. The mediator is resolved from context.RequestServices per request.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    public CustomerReviewQueryBuilder(IMediator mediator, IAuthorizationService authorizationService) : this(authorizationService)
    {
    }
}
