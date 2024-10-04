using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;
using static VirtoCommerce.XCart.Core.ModuleConstants;
using VirtoCommerce.Xapi.Core.Extensions;
using VirtoCommerce.Xapi.Core.Helpers;
using VirtoCommerce.Xapi.Core.Infrastructure;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
// using VirtoCommerce.XCart.Core.Schemas;
// using VirtoCommerce.XCart.Core;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateCustomerReviewCommandBuilder: CommandBuilder<CreateCustomerReviewCommand, CreateCustomerReviewPayload, CreateCustomerReviewCommandType, CustomerReviewType>
{
    protected override string Name => "createCustomerReview";

    public CreateCustomerReviewCommandBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }
}

// public class CustomerReviewsSchema : ISchemaBuilder
// {
//     private readonly IMediator _mediator;
//     private readonly IDistributedLockService _distributedLockService;
//
//     public const string CustomerReviewPrefix = "CustomerReview";
//
//     public CustomerReviewsSchema(IMediator mediator, IDistributedLockService distributedLockService)
//     {
//         _mediator = mediator;
//         _distributedLockService = distributedLockService;
//     }
//
//     public void Build(ISchema schema)
//     {
//         var addItemField = FieldBuilder.Create<CreateCustomerReviewPayload, CreateCustomerReviewPayload>(GraphTypeExtenstionHelper.GetActualType<CustomerReviewType>())
//             .Name("createCustomerReview")
//             .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<CreateCustomerReviewCommandType>>(), SchemaConstants.CommandName)
//             .ResolveSynchronizedAsync(CustomerReviewPrefix, "userId", _distributedLockService, async context =>
//             {
//                 // var cartCommand = context.GetCartCommand<AddCartItemCommand>();
//                 var command = (CreateCustomerReviewCommand)context.GetArgument(GenericTypeHelper.GetActualType<CreateCustomerReviewCommand>(), SchemaConstants.CommandName);
//
//                 // await CheckAuthByCartCommandAsync(context, cartCommand);
//                 
//
//                 //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
//                 var customerReviewPayload = await _mediator.Send(command);
//
//                 //store cart aggregate in the user context for future usage in the graph types resolvers
//                 context.SetExpandedObjectGraph(customerReviewPayload);
//                 return customerReviewPayload;
//             })
//             .FieldType;
//
//         schema.Mutation.AddField(addItemField);
//     }
// }
