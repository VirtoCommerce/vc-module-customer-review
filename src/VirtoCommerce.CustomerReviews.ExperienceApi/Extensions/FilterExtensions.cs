using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Extensions;

public static class FilterExtensions
{
    public static T Get<T>(this IList<IFilter> filters, string fieldName)
        where T: INamedFilter
    {
        return filters.OfType<T>().FirstOrDefault(x => x.FieldName.EqualsInvariant(fieldName));
    }
}
