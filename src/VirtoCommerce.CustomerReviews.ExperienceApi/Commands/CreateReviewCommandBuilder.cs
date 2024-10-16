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

    public CreateReviewCommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, CreateReviewCommand request)
    {
        request.UserId = context.GetCurrentUserId();

        await base.BeforeMediatorSend(context, request);
        await Authorize(context, request, new ReviewAuthorizationRequirement());
    }
}
