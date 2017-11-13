using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture.Xunit;
using VirtoCommerce.CustomReview.Data.Model;
using VirtoCommerce.CustomReview.Data.Services;
using Xunit;

namespace VirtoCommerce.CustomReview.Test
{
    public class Tests
    {
        [Fact]
        public void CanSaveReview()
        {
            var newReview = new Review
            {
                ProductId = "123",
                Text = "new text",
                Rate = 4,
                Author = "Иванов"
            };

            var reviewServiceMock = new Mock<IReviewServiceImpl>();

        }

        [Fact]
        public void CanUpdateReview()
        {
        }

        [Fact]
        public void GetProductReviews()
        {
        }

        [Fact]
        public void GetProductReviewsApprovedBy()
        {
        }

        [Fact]
        public void GetRateProduct()
        {
        }


        [Fact]
        public void CanDeleteReview()
        {
        }

    }

}
