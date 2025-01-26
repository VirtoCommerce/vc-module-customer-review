using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Queries;
using VirtoCommerce.FileExperienceApi.Core.Models;
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
        var authorized = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

        if (!authorized)
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            var currentUserId = GetUserId(context);

            switch (context.Resource)
            {
                case File:
                    authorized = true;
                    break;
                case CreateCustomerReviewCommand command:
                    authorized = isAuthenticated && command.UserId == currentUserId && await IsStoreAvailable(command.StoreId, currentUserId);
                    break;
                case CustomerReviewsQuery:
                    authorized = true;
                    break;
                case CreateReviewCommand command:
                    authorized = isAuthenticated && await IsStoreAvailable(command.StoreId, currentUserId);
                    break;
                case CanLeaveFeedbackQuery query:
                    authorized = isAuthenticated && await IsStoreAvailable(query.StoreId, currentUserId);
                    break;
            }
        }

        if (authorized)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private static string GetUserId(AuthorizationHandlerContext context)
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
