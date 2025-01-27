using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.FileExperienceApi.Core.Authorization;
using VirtoCommerce.FileExperienceApi.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Authorization;

public class CustomerReviewImageAuthorizationRequirementFactory : IFileAuthorizationRequirementFactory
{
    public string Scope => ModuleConstants.CustomerReviewImagesScope;

    public IAuthorizationRequirement Create(File file, string permission)
    {
        return new CustomerReviewAuthorizationRequirement();
    }
}
