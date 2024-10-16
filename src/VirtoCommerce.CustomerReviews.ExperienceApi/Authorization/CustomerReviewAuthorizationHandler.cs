using System;
using System.Security.Claims;
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
            var currentUserId = GetUserId(context);

            switch (context.Resource)
            {
                case CreateCustomerReviewCommand command:
                    result = isAuthenticated && command.UserId == currentUserId && await IsStoreAvailable(currentUserId, command.StoreId);
                    break;
                case CustomerReviewsQuery:
                    result = true;
                    break;
                case CreateReviewCommand command:
                    result = isAuthenticated && await IsStoreAvailable(currentUserId, command.StoreId);
                    break;
                case CanLeaveFeedbackQuery query:
                    result = isAuthenticated && await IsStoreAvailable(currentUserId, query.StoreId);
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

    private static string GetUserId(AuthorizationHandlerContext context)
    {
        return
            context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            context.User.FindFirstValue("name") ??
            AnonymousUser.UserName;
    }

    private async Task<bool> IsStoreAvailable(string userId, string storeId)
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

        return user.StoreId == store.Id || store.TrustedGroups?.Contains(user.StoreId) == true;
    }
}
