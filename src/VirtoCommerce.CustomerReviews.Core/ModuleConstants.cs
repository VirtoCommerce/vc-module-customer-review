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
                public const string Read = "customerReviews:read";
                public const string Update = "customerReviews:update";
                public const string RatingRead = "customerReviews:ratingRead";
                public const string RatingRecalc = "customerReviews:ratingRecalc";

                public static string[] AllPermissions = { Read, Update, RatingRead, RatingRecalc };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor CustomerReviewsEnabled = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsEnabled",
                    GroupName = "Store|CustomerReviews",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static SettingDescriptor CustomerReviewsEnabledForAnonymous = new SettingDescriptor
                {
                    Name = "CustomerReviews.CustomerReviewsEnabledForAnonymous",
                    GroupName = "Store|CustomerReviews",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static SettingDescriptor CanSubmitReviewWhenHasOrder = new SettingDescriptor
                {
                    Name = "CustomerReviews.CanSubmitReviewWhenHasOrder",
                    GroupName = "Store|CustomerReviews",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true,
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        return new List<SettingDescriptor>
                               {
                                   CustomerReviewsEnabled,
                                   CustomerReviewsEnabledForAnonymous,
                                   CanSubmitReviewWhenHasOrder
                               };
                    }
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
