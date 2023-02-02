using System;
using System.Linq;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Extensions;

public static class RangeFilterExtensions
{
    private delegate bool TryParse<T>(string str, out T result);

    public static void MapTo(this RangeFilter rangeFilter, Action<DateTime> setStart, Action<DateTime> setEnd)
    {
        rangeFilter.MapTo<DateTime>(DateTime.TryParse,
            (lower, includeLower) => setStart(lower + (includeLower ? TimeSpan.Zero : TimeSpan.MinValue)),
            (upper, includeUpper) => setEnd(upper - (includeUpper ? TimeSpan.Zero : TimeSpan.MinValue)));
    }

    public static void MapTo(this RangeFilter rangeFilter, Action<int> setStart, Action<int> setEnd)
    {
        rangeFilter.MapTo<int>(int.TryParse,
            (lower, includeLower) => setStart(lower + (includeLower ? 0 : 1)),
            (upper, includeUpper) => setEnd(upper - (includeUpper ? 0 : 1)));
    }

    private static void MapTo<T>(this RangeFilter rangeFilter, TryParse<T> tryParse, Action<T, bool> setStart, Action<T, bool> setEnd)
    {
        var range = rangeFilter?.Values?.FirstOrDefault();
        if (range != null)
        {
            if (tryParse(range.Lower, out var lower))
            {
                setStart(lower, range.IncludeLower);
            }

            if (tryParse(range.Upper, out var upper))
            {
                setEnd(upper, range.IncludeUpper);
            }
        }
    }
}
