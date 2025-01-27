angular.module('VirtoCommerce.CustomerReviews')
    .controller('VirtoCommerce.CustomerReviews.reviewImageWidgetController',
        ['$scope', 'platformWebApp.dialogService',
            function ($scope, dialogService) {
                var blade = $scope.blade;

                $scope.openBlade = function () {
                    if (blade.currentEntities && _.any(blade.currentEntities)) {
                        var dialog = {
                            images: blade.currentEntities,
                            currentImage: blade.currentEntities[0]
                        };
                        dialogService.showGalleryDialog(dialog);
                    }
                };

                function setCurrentEntities(images) {
                    if (images) {
                        blade.currentEntities = images;
                    }
                }
                $scope.$watch('blade.currentEntity.images', setCurrentEntities);
            }]);
