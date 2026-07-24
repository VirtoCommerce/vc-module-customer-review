using System;
using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.ExperienceApi.Authorization;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;
using VirtoCommerce.Xapi.Core.Extensions;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateReviewCommandBuilder : CommandBuilder<CreateReviewCommand, CreateReviewResult, CreateReviewCommandType, CreateReviewResultType>
{
    protected override string Name => "createReview";

    public CreateReviewCommandBuilder(IAuthorizationService authorizationService)
        : base(authorizationService)
    {
    }

    [Obsolete("Use the constructor without IMediator. The mediator is resolved from context.RequestServices per request.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
    public CreateReviewCommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : this(authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, CreateReviewCommand request)
    {
        request.UserId = context.GetCurrentUserId();

        await base.BeforeMediatorSend(context, request);
        await Authorize(context, request, new CustomerReviewAuthorizationRequirement());
    }
}
