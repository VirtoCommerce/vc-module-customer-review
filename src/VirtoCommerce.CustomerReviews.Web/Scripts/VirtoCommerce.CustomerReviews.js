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
    .run(['platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state',
        'platformWebApp.authService', 'VirtoCommerce.CustomerReviews.entityTypesResolverService',
        'virtoCommerce.catalogModule.items',
        function (mainMenuService, widgetService, $state, authService, entityTypesResolverService, items) {

        //Register reviews menu item
        var menuItemNewReviews = {
            path: 'browse/customerReviewsNew',
            icon: 'fa fa-comments',
            title: 'customerReviews.main-menu-title',
            priority: 100,
            action: function () { $state.go('workspace.customerReviews') },
            permission: 'customerReviews:read'
        };
        mainMenuService.addMenuItem(menuItemNewReviews);

        //Reviews widget inside product blade
        var itemReviewsWidget = {
            controller: 'VirtoCommerce.CustomerReviews.productRatingWidgetController',
            template: 'Modules/$(VirtoCommerce.CustomerReviews)/Scripts/widgets/product-rating-widget.tpl.html',
            isVisible: function (blade) { return authService.checkPermission('customerReviews:read'); }
        };
        widgetService.registerWidget(itemReviewsWidget, 'itemDetail');

        //Product entityType resolver
        entityTypesResolverService.registerType({
            entityType: 'Product',
            description: 'customerReviews.blades.product-detail.description',
            fullTypeName: 'VirtoCommerce.CatalogModule.Core.Model.CatalogProduct',
            icon: 'fas fa-shopping-bag',
            entityIdFieldName: 'itemId',
            detailBlade: {
                controller: 'virtoCommerce.catalogModule.itemDetailController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html'
            },
            getEntity: function (entityId, setEntityCallback) {
                items.get({ id: entityId, respGroup: 1 }, (data) => {
                    setEntityCallback(data.name, data.imgSrc);
                });
            },
            knownChildrenTypes: []
        });

    }
]);
