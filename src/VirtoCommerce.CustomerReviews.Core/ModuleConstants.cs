using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CustomerReviews.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string CustomerReviewRead = "customerReviews:read";
                public const string CustomerReviewUpdate = "customerReviews:update";
                public const string CustomerReviewDelete = "customerReviews:delete";
                public const string CustomerReviewRatingRead = "customerReviews:ratingRead";
                public const string CustomerReviewRatingRecalc = "customerReviews:ratingRecalc";

                public static string[] AllPermissions = { CustomerReviewRead, CustomerReviewUpdate, CustomerReviewDelete, CustomerReviewRatingRead, CustomerReviewRatingRecalc };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static readonly SettingDescriptor RequestReviewEnableJob = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsEnabledRequestReviewJob",
                    GroupName = "Product Reviews|Email Review Reminder",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static readonly SettingDescriptor RequestReviewCronJob = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsRequestReviewCronJob",
                    GroupName = "Product Reviews|Email Review Reminder",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "0/15 * * * *"
                };

                public static readonly SettingDescriptor RequestReviewDaysInState = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsRequestReviewDaysInState",
                    GroupName = "Product Reviews|Email Review Reminder",
                    ValueType = SettingValueType.PositiveInteger,
                    DefaultValue = 10
                };

                public static readonly SettingDescriptor RequestReviewOrderInState = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsRequestReviewOrderInState",
                    GroupName = "Product Reviews|Email Review Reminder",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "Completed"
                };

                public static readonly SettingDescriptor RequestReviewMaxRequests = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsRequestReviewMaxRequests",
                    GroupName = "Product Reviews|Email Review Reminder",
                    ValueType = SettingValueType.PositiveInteger,
                    DefaultValue = 2
                };

                public static readonly SettingDescriptor CustomerReviewsEnabled = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsEnabled",
                    GroupName = "Store|Product Reviews",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false,
                    IsPublic = true,
                };

                public static readonly SettingDescriptor CustomerReviewsEnabledForAnonymous = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsEnabledForAnonymous",
                    GroupName = "Store|Product Reviews",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false,
                    IsPublic = true,
                };

                public static readonly SettingDescriptor CanSubmitReviewWhenHasOrder = new SettingDescriptor
                {
                    Name = "CustomerReviews.CanSubmitReviewWhenHasOrder",
                    GroupName = "Store|Product Reviews",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true,
                    IsPublic = true,
                };

                public static readonly SettingDescriptor CalculationMethod = new SettingDescriptor
                {
                    Name = "CustomerReviews.Calculation.Method",
                    GroupName = "Product Reviews|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "Average",
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        return new List<SettingDescriptor>
                               {
                                   CustomerReviewsEnabled,
                                   CustomerReviewsEnabledForAnonymous,
                                   CanSubmitReviewWhenHasOrder,
                                   RequestReviewEnableJob,
                                   RequestReviewCronJob,
                                   RequestReviewDaysInState,
                                   RequestReviewOrderInState,
                                   RequestReviewMaxRequests
                               };
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> JobSettings
            {
                get
                {
                    yield return General.RequestReviewEnableJob;
                    yield return General.RequestReviewCronJob;
                    yield return General.RequestReviewDaysInState;
                    yield return General.RequestReviewOrderInState;
                    yield return General.RequestReviewMaxRequests;
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllSettings;
                }
            }
        }
    }
}
