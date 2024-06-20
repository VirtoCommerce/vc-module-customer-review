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
            var currentUserId = GetUserId(context);

            switch (context.Resource)
            {
                case CreateCustomerReviewCommand command:
                    var userManager = _userManagerFactory();
                    var currentUser = await userManager.FindByIdAsync(currentUserId);
                    var store = await _storeService.GetNoCloneAsync(command.StoreId);
                    var allowedStoreIds = new List<string>(store.TrustedGroups) { store.Id };
                    result = allowedStoreIds.Contains(currentUser.StoreId) && command.UserId == currentUserId;
                    break;
                case CustomerReviewsQuery:
                    result = true;
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
}
