angular.module('VirtoCommerce.CustomReview.Web')
.factory('VirtoCommerce.CustomReview.WebApi', ['$resource', function ($resource) {
    return $resource('api/VirtoCommerce.CustomReview.Web');
}]);
