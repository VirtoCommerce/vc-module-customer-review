using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CanLeaveFeedbackQuery : Query<bool>
{
    public string StoreId { get; set; }

    public string EntityId { get; set; }

    public string EntityType { get; set; }

    public string UserId { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(EntityId));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(EntityType));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(UserId));
    }

    public override void Map(IResolveFieldContext context)
    {
        StoreId = context.GetArgument<string>(nameof(StoreId));
        EntityId = context.GetArgument<string>(nameof(EntityId));
        EntityType = context.GetArgument<string>(nameof(EntityType));
        UserId = context.GetArgument<string>(nameof(UserId));
    }
}