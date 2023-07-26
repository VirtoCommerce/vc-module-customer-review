using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateCustomerReviewCommandHandler : IRequestHandler<CreateCustomerReviewCommand, CustomerReview>
{
    private readonly ICustomerReviewService _customerReviewService;
    private readonly IMapper _mapper;

    public CreateCustomerReviewCommandHandler(ICustomerReviewService customerReviewService, IMapper mapper)
    {
        _customerReviewService = customerReviewService;
        _mapper = mapper;
    }

    public async Task<CustomerReview> Handle(CreateCustomerReviewCommand request, CancellationToken cancellationToken)
    {
        var customerReview = _mapper.Map<CustomerReview>(request);
        await _customerReviewService.SaveChangesAsync(new[] { customerReview });
        return customerReview;
    }
}
