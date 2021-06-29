using System.Collections.Generic;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerReviews.Core.Events
{
    public class CustomerReviewChangeEvent : GenericChangedEntryEvent<CustomerReview>
    {
        public CustomerReviewChangeEvent(IEnumerable<GenericChangedEntry<CustomerReview>> changedEntries) : base(changedEntries)
        {
        }
    }
}
