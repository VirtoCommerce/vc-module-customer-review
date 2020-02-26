using System.Collections.Generic;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerReviews.Core.Events
{
    public class CustomerReviewChangedEvent : GenericChangedEntryEvent<CustomerReview>
    {
        public CustomerReviewChangedEvent(IEnumerable<GenericChangedEntry<CustomerReview>> changedEntries) : base(changedEntries)
        {
        }
    }
}
