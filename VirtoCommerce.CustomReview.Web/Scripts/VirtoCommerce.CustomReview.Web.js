//Call this to register our module to main application
var moduleTemplateName = "VirtoCommerce.CustomReview.Web";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleTemplateName);
}

angular.module(moduleTemplateName, [])
.config(['$stateProvider', '$urlRouterProvider',
    function ($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('workspace.VirtoCommerce.CustomReview.Web', {
                url: '/VirtoCommerce.CustomReview.Web',
                templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                controller: [
                    '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                        var newBlade = {
                            id: 'blade1',
                            controller: 'VirtoCommerce.CustomReview.Web.blade1Controller',
                            template: 'Modules/$(VirtoCommerce.CustomReview.Web)/Scripts/blades/helloWorld_blade1.tpl.html',
                            isClosingDisabled: true
                        };
                        bladeNavigationService.showBlade(newBlade);
                    }
                ]
            });
    }
])
.run(['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state',
    function ($rootScope, mainMenuService, widgetService, $state) {
        //Register module in main menu
        var menuItem = {
            path: 'browse/VirtoCommerce.CustomReview.Web',
            icon: 'fa fa-cube',
            title: 'VirtoCommerce.CustomReview.Web',
            priority: 100,
            action: function () { $state.go('workspace.VirtoCommerce.CustomReview.Web') },
            permission: 'VirtoCommerce.CustomReview.WebPermission'
        };
        mainMenuService.addMenuItem(menuItem);
    }
]);
