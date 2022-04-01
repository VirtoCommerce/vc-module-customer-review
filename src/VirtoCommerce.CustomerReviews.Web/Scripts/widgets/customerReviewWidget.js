angular.module('VirtoCommerce.CustomerReviews')
    .controller('VirtoCommerce.CustomerReviews.customerReviewWidgetController', ['$scope', 'CustomerReviews.WebApi', 'platformWebApp.bladeNavigationService', function ($scope, reviewsApi, bladeNavigationService) {
        var blade = $scope.blade;
        var filter = {
            take: 0,
            reviewStatus: null
        };

        function refresh() {
            $scope.loading = true;
            reviewsApi.search(filter, function (data) {
                $scope.loading = false;
                $scope.totalCount = data.totalCount;
            });
        }

        $scope.openBlade = function () {
            if ($scope.loading || !$scope.totalCount)
                return;

            var newBlade = {
                id: "reviewsList",
                filter: filter,
                title: 'Product Reviews for "' + blade.title + '"',
                controller: 'VirtoCommerce.CustomerReviews.reviewsListController',
                template: 'Modules/$(VirtoCommerce.CustomerReviews)/Scripts/blades/reviews-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.$watch("blade.itemId", function (id) {
            filter.productIds = [id];

            if (id) refresh();
        });
    }]);
