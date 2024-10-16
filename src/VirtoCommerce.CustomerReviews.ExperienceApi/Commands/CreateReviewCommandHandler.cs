using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Validators;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, CreateReviewResult>
{
    private readonly IMemberService _memberService;
    private readonly ICustomerReviewService _reviewService;
    private readonly IMapper _mapper;
    private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
    private readonly ReviewValidator _reviewValidator;

    public CreateReviewCommandHandler(
        ICustomerReviewService reviewService,
        IMapper mapper,
        IMemberService memberService,
        Func<UserManager<ApplicationUser>> userManagerFactory,
        ReviewValidator reviewValidator)
    {
        _reviewService = reviewService;
        _mapper = mapper;
        _memberService = memberService;
        _userManagerFactory = userManagerFactory;
        _reviewValidator = reviewValidator;
    }

    public async Task<CreateReviewResult> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var response = AbstractTypeFactory<CreateReviewResult>.TryCreateInstance();
        var validationResult = await _reviewValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            response.ValidationErrors.AddRange(validationResult.Errors.OfType<ReviewValidationError>());
        }
        else
        {
            var review = _mapper.Map<CustomerReview>(request);
            var userManager = _userManagerFactory();
            var currentUser = await userManager.FindByIdAsync(request.UserId);

            if (!string.IsNullOrEmpty(currentUser?.MemberId))
            {
                var contact = await _memberService.GetByIdAsync(currentUser.MemberId) as Contact;
                review.UserName = contact?.FullName ?? currentUser.UserName;
            }

            await _reviewService.SaveChangesAsync([review]);

            response.Id = review.Id;
            response.UserName = review.UserName;
        }

        return response;
    }
}
