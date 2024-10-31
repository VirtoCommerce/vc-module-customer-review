using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Queries;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;
using static VirtoCommerce.Xapi.Core.ModuleConstants;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Authorization;

public class CustomerReviewAuthorizationRequirement : IAuthorizationRequirement
{
}

public class CustomerReviewAuthorizationHandler : AuthorizationHandler<CustomerReviewAuthorizationRequirement>
{
    private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
    private readonly IStoreService _storeService;

    public CustomerReviewAuthorizationHandler(Func<UserManager<ApplicationUser>> userManagerFactory, IStoreService storeService)
    {
        _userManagerFactory = userManagerFactory;
        _storeService = storeService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomerReviewAuthorizationRequirement requirement)
    {
        var result = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

        if (!result)
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            var currentUserId = GetCurrentUserId(context);

            switch (context.Resource)
            {
                case CreateCustomerReviewCommand command:
                    result = isAuthenticated && command.UserId == currentUserId && await IsStoreAvailable(command.StoreId, currentUserId);
                    break;
                case CustomerReviewsQuery:
                    result = true;
                    break;
                case CreateReviewCommand command:
                    result = isAuthenticated && await IsStoreAvailable(command.StoreId, currentUserId);
                    break;
                case CanLeaveFeedbackQuery query:
                    result = isAuthenticated && await IsStoreAvailable(query.StoreId, currentUserId);
                    break;
            }
        }

        if (result)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private static string GetCurrentUserId(AuthorizationHandlerContext context)
    {
        return context.User.GetUserId() ?? AnonymousUser.UserName;
    }

    private async Task<bool> IsStoreAvailable(string storeId, string userId)
    {
        var store = await _storeService.GetNoCloneAsync(storeId);

        if (store == null)
        {
            return false;
        }

        var userManager = _userManagerFactory();
        var user = await userManager.FindByIdAsync(userId);

        if (string.IsNullOrEmpty(user?.StoreId))
        {
            return false;
        }

        return store.Id.EqualsIgnoreCase(user.StoreId) ||
               store.TrustedGroups?.Any(x => x.EqualsIgnoreCase(user.StoreId)) == true;
    }
}
