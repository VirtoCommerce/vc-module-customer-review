using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Validators;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateCustomerReviewCommandHandler : IRequestHandler<CreateCustomerReviewCommand, CreateCustomerReviewPayload>
{
    private readonly ICustomerReviewService _customerReviewService;
    private readonly IMapper _mapper;
    private readonly CustomerReviewValidator _customerReviewValidator;

    public CreateCustomerReviewCommandHandler(ICustomerReviewService customerReviewService, IMapper mapper, CustomerReviewValidator customerReviewValidator)
    {
        _customerReviewService = customerReviewService;
        _mapper = mapper;
        _customerReviewValidator = customerReviewValidator;
    }

    public async Task<CreateCustomerReviewPayload> Handle(CreateCustomerReviewCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _customerReviewValidator.ValidateAsync(request);
        var customerReview = _mapper.Map<CustomerReview>(request);
        var response = AbstractTypeFactory<CreateCustomerReviewPayload>.TryCreateInstance();

        response.CustomerReview = customerReview;

        if (!validationResult.IsValid)
        {
            response.ValidationErrors.AddRange(validationResult.Errors.OfType<CustomerReviewValidationError>());
        }
        else
        {
            await _customerReviewService.SaveChangesAsync([customerReview]);
        }

        return response;
    }
}
