angular.module('VirtoCommerce.CustomerReviews')
.factory('CustomerReviews.WebApi', ['$resource', function ($resource) {
    return $resource('api/customerReviews', {}, {
        search: { method: 'POST', url: 'api/customerReviews/reviewList' },
        update: { method: 'POST' },
        delete: { method: 'DELETE' },
        approve: { method: 'POST', url: 'api/customerReviews/approve' },
        reject: { method: 'POST', url: 'api/customerReviews/reject' },
        resetReviewStatus: { method: 'POST', url: 'api/customerReviews/reset' },
    });
    }]);
