angular.module('VirtoCommerce.CustomerReviews')
    .controller('VirtoCommerce.CustomerReviews.reviewsListController', ['$scope', 'CustomerReviews.WebApi', 'platformWebApp.bladeUtils', 'uiGridConstants', 'platformWebApp.uiGridHelper', 'platformWebApp.authService', 'VirtoCommerce.CustomerReviews.entityTypesResolverService',
        function ($scope, reviewsApi, bladeUtils, uiGridConstants, uiGridHelper, authService, entityTypesResolverService) {
            $scope.uiGridConstants = uiGridConstants;

            var blade = $scope.blade;
            var bladeNavigationService = bladeUtils.bladeNavigationService;
            

            blade.refresh = function () {
                blade.isLoading = true;
                var reviewStatus = filter.reviewStatusObj ? filter.reviewStatusObj.value : null;
                var statuses = null;
                if (reviewStatus != null) {
                    statuses = [reviewStatus];
                }
                reviewsApi.search(angular.extend(filter, {
                    searchPhrase: filter.keyword ? filter.keyword : undefined,
                    reviewStatus: statuses,
                    sort: uiGridHelper.getSortExpression($scope),
                    skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                    take: $scope.pageSettings.itemsPerPageCount
                }), function (data) {
                    blade.isLoading = false;
                    $scope.pageSettings.totalItems = data.totalCount;
                    blade.currentEntities = data.results;
                });
            }

            blade.selectNode = function (data) {
                $scope.selectedNodeId = data.id;

                if (!authService.checkPermission('customerReviews:update')) {
                    return;
                }

                var newBlade = {
                    id: 'reviewDetails',
                    currentEntityId: data.id,
                    currentEntity: angular.copy(data),
                    title: 'Customer review',
                    controller: 'VirtoCommerce.CustomerReviews.reviewDetailController',
                    template: 'Modules/$(VirtoCommerce.CustomerReviews)/Scripts/blades/review-detail.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade);
            }    
            
            blade.headIcon = 'fa fa-comments';
            blade.title = 'Rating and Reviews';

            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh", icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                }
            ];

            $scope.reviewStatusList = [
                {
                    name: "New",
                    value: 0
                },
                {
                    name: "Approved",
                    value: 1
                },
                {
                    name: "Rejected",
                    value: 2
                }
            ];

            $scope.entityTypesList = entityTypesResolverService.objects.map(x => x.entityType);

            // simple and advanced filtering
            var filter = $scope.filter = blade.filter || {};

            if (filter.reviewStatus >= 0) {
                filter.reviewStatusObj = $scope.reviewStatusList[filter.reviewStatus];
            }

            filter.criteriaChanged = function () {
                if ($scope.pageSettings.currentPage > 1) {
                    $scope.pageSettings.currentPage = 1;
                } else {
                    blade.refresh();
                }
            };

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {
                uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                    uiGridHelper.bindRefreshOnSortChanged($scope);
                });
                bladeUtils.initializePagination($scope);
            };

        }]);
