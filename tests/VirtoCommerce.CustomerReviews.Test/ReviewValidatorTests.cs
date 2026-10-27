using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Queries;
using VirtoCommerce.CustomerReviews.ExperienceApi.Validators;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Settings;
using Xunit;

namespace VirtoCommerce.CustomerReviews.Test
{
    [Trait("Category", "UnitTest")]
    public class ReviewValidatorTests
    {
        private const string _productId = "P1";

        private readonly Mock<ICustomerReviewSearchService> _reviewSearchService = new();
        private readonly Mock<ICustomerOrderSearchService> _orderSearchService = new();
        private readonly Mock<ISettingsManager> _settingsManager = new();

        public ReviewValidatorTests()
        {
            _orderSearchService
                .Setup(x => x.SearchAsync(It.IsAny<CustomerOrderSearchCriteria>(), It.IsAny<bool>()))
                .ReturnsAsync(new CustomerOrderSearchResult { TotalCount = 1 });

            _reviewSearchService
                .Setup(x => x.SearchAsync(It.IsAny<CustomerReviewSearchCriteria>(), It.IsAny<bool>()))
                .ReturnsAsync(new CustomerReviewSearchResult { TotalCount = 0 });

            _settingsManager
                .Setup(x => x.GetObjectSettingAsync(ModuleConstants.Settings.General.ReviewMaximumImages.Name, null, null))
                .ReturnsAsync(new ObjectSettingEntry(ModuleConstants.Settings.General.ReviewMaximumImages) { Value = 5 });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Validate_EmptyEntityId_ReturnsEntityIdIsEmptyError(string entityId)
        {
            var result = await CreateValidator().ValidateAsync(CreateCommand(entityId), TestContext.Current.CancellationToken);

            Assert.False(result.IsValid);
            var errorCodes = result.Errors.OfType<ReviewValidationError>().Select(x => x.ErrorCode).ToList();
            Assert.Contains("ENTITY_ID_IS_EMPTY", errorCodes);
            Assert.DoesNotContain("PRODUCT_MUST_BE_PURCHASED", errorCodes);
        }

        [Fact]
        public async Task Validate_PurchasedProduct_IsValid()
        {
            var result = await CreateValidator().ValidateAsync(CreateCommand(_productId), TestContext.Current.CancellationToken);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_NotPurchasedProduct_ReturnsProductMustBePurchasedError()
        {
            _orderSearchService
                .Setup(x => x.SearchAsync(It.IsAny<CustomerOrderSearchCriteria>(), It.IsAny<bool>()))
                .ReturnsAsync(new CustomerOrderSearchResult { TotalCount = 0 });

            var result = await CreateValidator().ValidateAsync(CreateCommand(_productId), TestContext.Current.CancellationToken);

            Assert.Contains("PRODUCT_MUST_BE_PURCHASED", result.Errors.OfType<ReviewValidationError>().Select(x => x.ErrorCode));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task OrderExists_EmptyEntityId_ReturnsFalseWithoutSearch(string entityId)
        {
            var result = await CreateValidator().OrderExists(CreateCommand(entityId));

            Assert.False(result);
            _orderSearchService.Verify(x => x.SearchAsync(It.IsAny<CustomerOrderSearchCriteria>(), It.IsAny<bool>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CanLeaveFeedback_EmptyEntityId_ReturnsFalse(string entityId)
        {
            var handler = new CanLeaveFeedbackQueryHandler(CreateValidator());

            var result = await handler.Handle(CreateQuery(entityId), TestContext.Current.CancellationToken);

            Assert.False(result);
        }

        [Fact]
        public async Task CanLeaveFeedback_PurchasedProduct_ReturnsTrue()
        {
            var handler = new CanLeaveFeedbackQueryHandler(CreateValidator());

            var result = await handler.Handle(CreateQuery(_productId), TestContext.Current.CancellationToken);

            Assert.True(result);
        }

        private ReviewValidator CreateValidator()
        {
            return new ReviewValidator(_reviewSearchService.Object, _orderSearchService.Object, _settingsManager.Object);
        }

        private static CreateReviewCommand CreateCommand(string entityId)
        {
            return new CreateReviewCommand
            {
                StoreId = "Store1",
                EntityId = entityId,
                EntityType = ReviewEntityTypes.Product,
                UserId = "User1",
                Review = "Great product",
                Rating = 5,
            };
        }

        private static CanLeaveFeedbackQuery CreateQuery(string entityId)
        {
            return new CanLeaveFeedbackQuery
            {
                StoreId = "Store1",
                EntityId = entityId,
                EntityType = ReviewEntityTypes.Product,
                UserId = "User1",
            };
        }
    }
}
