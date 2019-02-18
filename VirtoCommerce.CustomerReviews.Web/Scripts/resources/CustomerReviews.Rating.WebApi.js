angular.module('VirtoCommerce.CustomerReviews')
.factory('VirtoCommerce.RatingApi', ['$resource', function ($resource) {
    return $resource('api/rating', {},
        {
            calculateStore: { method: 'POST', url: 'api/rating/calculateStore' },
            get: { method: 'POST', url: 'api/rating/productRatingInCatalog', isArray: true }
        });
}]);