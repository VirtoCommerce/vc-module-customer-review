using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;
using VirtoCommerce.Xapi.Core.BaseQueries;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateCustomerReviewCommandBuilder: CommandBuilder<CreateCustomerReviewCommand, CreateCustomerReviewPayload, CreateCustomerReviewCommandType, CustomerReviewType>
{
    protected override string Name => "createCustomerReview";

    public CreateCustomerReviewCommandBuilder(IMediator mediator, IAuthorizationService authorizationService) : base(mediator, authorizationService)
    {
    }
}
