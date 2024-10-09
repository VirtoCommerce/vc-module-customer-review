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

public class CreateCustomerReviewCommandHandler : IRequestHandler<CreateCustomerReviewCommand, CreateReviewResult>
{
    private readonly ICustomerReviewService _customerReviewService;
    private readonly IMapper _mapper;
    private readonly ReviewValidator _reviewValidator;

    public CreateCustomerReviewCommandHandler(ICustomerReviewService customerReviewService, IMapper mapper, ReviewValidator reviewValidator)
    {
        _customerReviewService = customerReviewService;
        _mapper = mapper;
        _reviewValidator = reviewValidator;
    }

    public async Task<CreateReviewResult> Handle(CreateCustomerReviewCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _reviewValidator.ValidateAsync(request);
        var customerReview = _mapper.Map<CustomerReview>(request);
        var response = AbstractTypeFactory<CreateReviewResult>.TryCreateInstance();

        response.Review = customerReview;

        if (!validationResult.IsValid)
        {
            response.ValidationErrors.AddRange(validationResult.Errors.OfType<ReviewValidationError>());
        }
        else
        {
            await _customerReviewService.SaveChangesAsync([customerReview]);
        }

        return response;
    }
}
