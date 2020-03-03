using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerReviews.Data.Handlers
{
    public class ReviewStatusChangedEventHandler : IEventHandler<ReviewStatusChangedEvent>
    {
        private readonly IRatingService _ratingService;

        public ReviewStatusChangedEventHandler(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        public Task Handle(ReviewStatusChangedEvent message)
        {
            return _ratingService.CalculateAsync(message.ChangedEntries.Select(x => x.NewEntry).ToArray());
        }
    }
}
