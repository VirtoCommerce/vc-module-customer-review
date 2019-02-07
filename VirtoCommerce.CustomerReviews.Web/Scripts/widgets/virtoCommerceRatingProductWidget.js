angular.module('VirtoCommerce.CustomerReviews')
    .controller('VirtoCommerce.CustomerReviews.ratingProductWidgetController',
        ['$scope', 'VirtoCommerce.RatingApi', 'platformWebApp.bladeNavigationService',
            function ($scope, ratingApi, bladeNavigationService) {

                var blade = $scope.blade;

                function init() {
                    $scope.loading = false;
                    $scope.ratingRange = '0';
                    $scope.ratings = null;
                }

                function refresh() {
                    $scope.loading = true;

                    var params = {
                        productIds: [blade.itemId],
                        catalogId: blade.catalog.id
                    };

                    ratingApi.get(params, function (data) {
                        $scope.loading = false;
                        $scope.ratings = data;
                        $scope.ratingRange = getRatingRange(data);
                    });
                }

                function getRatingRange(ratings) {
                    var ratingValues = ratings.map(s => s.value);

                    if (ratings.length === 1) {
                        return ratingValues[0].toFixed(1);
                    }

                    if ($scope.ratings.length > 1) {
                        var max = Math.max(...ratingValues).toFixed(1);
                        var min = Math.min(...ratingValues).toFixed(1);

                        if (min === max) {
                            return `${min}`;
                        }

                        return `${min}-${max}`;
                    }

                    return 0;
                }

                /*$scope.openBlade = function () {
                    if ($scope.loading) return;

                    var newBlade = {
                        id: "rating_product_blade",
                        ratings: $scope.ratings,
                        title: 'Rating for "' + blade.title + '"',
                        controller: 'VirtoCommerce.CustomerReviews.productBladeController',
                        template: 'Modules/$(VirtoCommerce.CustomerReviews)/Scripts/blades/product/product_blade.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                };*/

                $scope.$watch("blade.itemId", function (id) {
                    if (id) refresh();
                });

                init();

            }]);
