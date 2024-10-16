using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.ExperienceApi.Authorization;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CanLeaveFeedbackQueryBuilder : QueryBuilder<CanLeaveFeedbackQuery, bool, BooleanGraphType>
{
    protected override string Name => "canLeaveFeedback";

    public CanLeaveFeedbackQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, CanLeaveFeedbackQuery request)
    {
        await base.BeforeMediatorSend(context, request);
        await Authorize(context, request, new ReviewAuthorizationRequirement());
    }
}
