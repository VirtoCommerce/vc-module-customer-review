using System.Collections.Generic;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerReviews.Core.Events
{
    public class ReviewStatusChangedEvent : GenericChangedEntryEvent<ReviewStatusChangeData>
    {
        public ReviewStatusChangedEvent(IEnumerable<GenericChangedEntry<ReviewStatusChangeData>> changedEntries) : base(changedEntries)
        {
        }
    }
}
