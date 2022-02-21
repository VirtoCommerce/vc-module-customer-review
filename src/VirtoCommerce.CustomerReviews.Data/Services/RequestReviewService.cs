using System;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Repositories;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class RequestReviewService : IRequestReviewService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;

        public RequestReviewService(Func<ICustomerReviewRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task MarkAccessRequest(string[] requestIds)
        {
            using (var repository = _repositoryFactory())
            {
                var requests = await repository.GetRequestReviewByIdAsync(requestIds);
                foreach (var request in requests)
                {
                    request.AccessDate = DateTime.Now;
                }
                await repository.UnitOfWork.CommitAsync();
            }
        }
    }
}
