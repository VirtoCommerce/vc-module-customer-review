angular.module('VirtoCommerce.CustomReview.Web')
.controller('VirtoCommerce.CustomReview.Web.blade1Controller', ['$scope', 'VirtoCommerce.CustomReview.WebApi', function ($scope, api) {
    var blade = $scope.blade;
    blade.title = 'VirtoCommerce.CustomReview.Web';

    blade.refresh = function () {
        api.get(function (data) {
            blade.data = data.result;
            blade.isLoading = false;
        });
    }

    blade.refresh();
}]);
