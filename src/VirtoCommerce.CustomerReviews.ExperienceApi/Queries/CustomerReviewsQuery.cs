using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CustomerReviewsQuery: SearchQuery<CustomerReviewSearchResult>
{
    public string StoreId { get; set; }

    public string EntityId { get; set; }

    public string EntityType { get; set; }

    public string Filter { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        foreach (var argument in base.GetArguments())
        {
            yield return argument;
        }

        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(EntityId));
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(EntityType));
        yield return Argument<StringGraphType>(nameof(Filter));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        StoreId = context.GetArgument<string>(nameof(StoreId));
        EntityId = context.GetArgument<string>(nameof(EntityId));
        EntityType = context.GetArgument<string>(nameof(EntityType));
        Filter = context.GetArgument<string>(nameof(Filter));
    }
}
