using System;
using System.Collections.Generic;
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

public class ReviewAuthorizationRequirement : IAuthorizationRequirement
{
}

public class ReviewAuthorizationHandler : AuthorizationHandler<ReviewAuthorizationRequirement>
{
    private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
    private readonly IStoreService _storeService;

    public ReviewAuthorizationHandler(Func<UserManager<ApplicationUser>> userManagerFactory, IStoreService storeService)
    {
        _userManagerFactory = userManagerFactory;
        _storeService = storeService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ReviewAuthorizationRequirement requirement)
    {
        var result = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

        if (!result)
        {
            var currentUserId = GetUserId(context);

            switch (context.Resource)
            {
                case CreateCustomerReviewCommand command:
                    result = context.User.Identity.IsAuthenticated && command.UserId == currentUserId && await IsStoreAvailable(currentUserId, command.StoreId);
                    break;
                case CustomerReviewsQuery:
                    result = true;
                    break;
                case CreateReviewCommand createCommand:
                    result = context.User.Identity.IsAuthenticated && await IsStoreAvailable(currentUserId, createCommand.StoreId);
                    break;
                case CanLeaveFeedbackQuery query:
                    result = context.User.Identity.IsAuthenticated && await IsStoreAvailable(currentUserId, query.StoreId);
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

        var allowedStoreIds = new List<string>(store.TrustedGroups) { store.Id };
        var userManager = _userManagerFactory();
        var currentUser = await userManager.FindByIdAsync(userId);

        return allowedStoreIds.Contains(currentUser?.StoreId);
    }
}
