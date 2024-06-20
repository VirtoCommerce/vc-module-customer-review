using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateCustomerReviewCommandBuilder: CommandBuilder<CreateCustomerReviewCommand, CustomerReview, CreateCustomerReviewCommandType, CustomerReviewType>
{
    protected override string Name => "createCustomerReview";

    public CreateCustomerReviewCommandBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }
}
