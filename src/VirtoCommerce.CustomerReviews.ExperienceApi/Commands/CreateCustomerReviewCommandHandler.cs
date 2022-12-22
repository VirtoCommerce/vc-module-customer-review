using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateCustomerReviewCommandHandler: IRequestHandler<CreateCustomerReviewCommand, CustomerReview>
{
    private readonly ICrudService<CustomerReview> _customerReviewCrudService;
    private readonly IMapper _mapper;

    public CreateCustomerReviewCommandHandler(ICrudService<CustomerReview> customerReviewCrudService, IMapper mapper)
    {
        _customerReviewCrudService = customerReviewCrudService;
        _mapper = mapper;
    }

    public async Task<CustomerReview> Handle(CreateCustomerReviewCommand request, CancellationToken cancellationToken)
    {
        var customerReview = _mapper.Map<CustomerReview>(request);
        await _customerReviewCrudService.SaveChangesAsync(new[] { customerReview });
        return customerReview;
    }
}
