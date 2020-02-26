using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerReviews.Core.Events
{
    public class ReviewStatusChangedEvent : DomainEvent
    {
        public ReviewStatusChangedEvent(IEnumerable<ReviewStatusChangeData> data)
        {
            Data = data;
        }

        public IEnumerable<ReviewStatusChangeData> Data { get; private set; }
    }
}
