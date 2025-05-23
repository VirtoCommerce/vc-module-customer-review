using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Validators;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.FileExperienceApi.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, CreateReviewResult>
{
    private readonly IMemberService _memberService;
    private readonly ICustomerReviewService _reviewService;
    private readonly IMapper _mapper;
    private readonly IFileUploadService _fileUploadService;
    private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
    private readonly ReviewValidator _reviewValidator;

    private const string _attachmentsUrlPrefix = "/api/files/";
    private readonly StringComparer _ignoreCase = StringComparer.OrdinalIgnoreCase;

    public CreateReviewCommandHandler(
        ICustomerReviewService reviewService,
        IMapper mapper,
        IMemberService memberService,
        IFileUploadService fileUploadService,
        Func<UserManager<ApplicationUser>> userManagerFactory,
        ReviewValidator reviewValidator)
    {
        _reviewService = reviewService;
        _mapper = mapper;
        _memberService = memberService;
        _fileUploadService = fileUploadService;
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

            IList<File> files = null;
            if (!request.ImageUrls.IsNullOrEmpty())
            {
                files = await SaveImages(request, review);
            }

            await _reviewService.SaveChangesAsync([review]);

            response.Id = review.Id;
            response.UserName = review.UserName;

            if (files?.Count > 0)
            {
                foreach (var file in files)
                {
                    file.OwnerEntityId = review.Id;
                    file.OwnerEntityType = nameof(CustomerReview);
                }

                await _fileUploadService.SaveChangesAsync(files);
            }
        }

        return response;
    }

    protected virtual async Task<IList<File>> SaveImages(CreateReviewCommand request, CustomerReview review)
    {
        var files = await GetFiles(request.ImageUrls);

        var filesByUrls = files
            .Where(x => x.Scope == ModuleConstants.CustomerReviewImagesScope &&
                        (
                            string.IsNullOrEmpty(x.OwnerEntityId) && string.IsNullOrEmpty(x.OwnerEntityType) ||
                            x.OwnerEntityId == review.Id && x.OwnerEntityType == nameof(CustomerReview)
                        )
            )
            .ToDictionary(x => GetFileUrl(x.Id), _ignoreCase);

        files = [];
        review.Images = [];

        foreach (var url in request.ImageUrls)
        {
            if (filesByUrls.TryGetValue(url, out var file))
            {
                review.Images.Add(ConvertToReviewImage(file));
                files.Add(file);
            }
        }

        return files;
    }

    protected virtual async Task<IList<File>> GetFiles(IList<string> urls)
    {
        var ids = urls
            .Select(GetFileId)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        var files = await _fileUploadService.GetAsync(ids);

        return files;
    }

    protected virtual CustomerReviewImage ConvertToReviewImage(File file)
    {
        var reviewImage = AbstractTypeFactory<CustomerReviewImage>.TryCreateInstance();

        reviewImage.Name = file.Name;
        reviewImage.Url = GetFileUrl(file.Id);

        return reviewImage;
    }

    protected static string GetFileUrl(string id)
    {
        return $"{_attachmentsUrlPrefix}{id}";
    }

    protected static string GetFileId(string url)
    {
        return url != null && url.StartsWith(_attachmentsUrlPrefix)
            ? url[_attachmentsUrlPrefix.Length..]
            : null;
    }
}
