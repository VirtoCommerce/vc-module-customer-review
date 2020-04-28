//Call this to register our module to main application
var moduleTemplateName = "VirtoCommerce.CustomerReviews";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleTemplateName);
}

angular.module(moduleTemplateName, [])
.config(['$stateProvider', '$urlRouterProvider',
    function ($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('workspace.customerReviews', {
                url: '/customerReviews',
                templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                controller: [
                    '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                        var newBlade = {
                            id: 'reviewsList',
                            controller: 'VirtoCommerce.CustomerReviews.reviewsListController',
                            template: 'Modules/$(VirtoCommerce.CustomerReviews)/Scripts/blades/reviews-list.tpl.html',
                            isClosingDisabled: true,
                            //filter: { reviewStatus: null }
                        };
                        bladeNavigationService.showBlade(newBlade);
                    }
                ]
            });
            
    }
])
    .run(['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'platformWebApp.authService',
    function ($rootScope, mainMenuService, widgetService, $state, authService) {

        //Register reviews menu item
        var menuItemNewReviews = {
            path: 'browse/customerReviewsNew',
            icon: 'fa fa-comments',
            title: 'Customer Reviews',
            priority: 100,
            action: function () { $state.go('workspace.customerReviews') },
            permission: 'customerReviews:read'
        };
        mainMenuService.addMenuItem(menuItemNewReviews);

        //Register reviews widget inside product blade
        var itemReviewsWidget = {
            controller: 'VirtoCommerce.CustomerReviews.customerReviewWidgetController',
            template: 'Modules/$(VirtoCommerce.CustomerReviews)/Scripts/widgets/customerReviewWidget.tpl.html',
            isVisible: function (blade) { return authService.checkPermission('customerReviews:read'); }
        };
        widgetService.registerWidget(itemReviewsWidget, 'itemDetail');

        //Register rating widget inside product blade
        var ratingWidget = {
            controller: 'VirtoCommerce.CustomerReviews.ratingProductWidgetController',
            template: 'modules/$(VirtoCommerce.CustomerReviews)/Scripts/widgets/virtoCommerceRatingProductWidget.tpl.html',
            isVisible: function (blade) { return authService.checkPermission('customerReviews:ratingRead'); }
        };
        widgetService.registerWidget(ratingWidget, 'itemDetail');
        
    }
]);
