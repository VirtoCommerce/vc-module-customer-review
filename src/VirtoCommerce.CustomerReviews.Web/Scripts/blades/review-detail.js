angular.module('VirtoCommerce.CustomerReviews')
    .controller('VirtoCommerce.CustomerReviews.reviewDetailController', ['$rootScope', '$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings', 'virtoCommerce.catalogModule.categories', 'CustomerReviews.WebApi', 'platformWebApp.metaFormsService', function ($rootScope, $scope, bladeNavigationService, settings, categories, reviewsAPI, metaFormsService) {
        var blade = $scope.blade;
        blade.updatePermission = 'customerReviews:update';

        blade.isLoading = false;

        blade.origEntity = blade.currentEntity;
        blade.securityScopes = blade.currentEntity.securityScopes;
        blade.currentEntity = angular.copy(blade.currentEntity);

        blade.ratingValidator = function (value) {
            var pattern = new RegExp("^([1-5])$");
            return pattern.test(value);
        };

        blade.formScope = null;
        $scope.setForm = function (form) { blade.formScope = form; };
  
        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity) /*&& blade.hasUpdatePermission()*/;
        };

        function canSave() {
            return isDirty() && blade.formScope && blade.formScope.$valid;
        }

        function saveChanges() {
            blade.isLoading = true;
            reviewsAPI.update({}, [blade.currentEntity],
                function (data, headers) {
                    blade.isLoading = false;
                    blade.parentBlade.refresh(true);
                    blade.origEntity = blade.currentEntity;
                    $scope.bladeClose();
                },
                function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                    blade.isLoading = false;
                });
        };

        function approveReview() {
            reviewsAPI.approve({}, [blade.currentEntity.id],
                function (data, headers) {
                    blade.isLoading = false;
                    blade.parentBlade.refresh(true);
                    blade.origEntity = blade.currentEntity;
                    $scope.bladeClose();
                },
                function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                    blade.isLoading = false;
                });
        }

        function rejectReview() {
            reviewsAPI.reject({}, [blade.currentEntity.id],
                function (data, headers) {
                    blade.isLoading = false;
                    blade.parentBlade.refresh(true);
                    blade.origEntity = blade.currentEntity;
                    $scope.bladeClose();
                },
                function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                    blade.isLoading = false;
                });
        }

        function resetReviewStatus() {
            reviewsAPI.resetReviewStatus({}, [blade.currentEntity.id],
                function (data, headers) {
                    blade.isLoading = false;
                    blade.parentBlade.refresh(true);
                    blade.currentEntity.reviewStatusId = 0;
                    blade.origEntity = blade.currentEntity;
                },
                function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                    blade.isLoading = false;
                });
        }

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, saveChanges, closeCallback, "customerReviews.dialogs.customerReview-save.title", "customerReviews.dialogs.customerReview-save.message");
        };

        function canApprove() {
            return blade.currentEntity.reviewStatusId === 0;
        }

        function canReject() {
            return blade.currentEntity.reviewStatusId === 0;
        }

        function canReset() {
            return blade.currentEntity.reviewStatusId !== 0;
        }

        blade.toolbarCommands = [
            {
                name: "customerReviews.blades.review-detail.commands.approve", icon: 'fa fa-edit',
                executeMethod: approveReview,
                canExecuteMethod: canApprove,
                permission: blade.updatePermission
            },
            {
                name: "customerReviews.blades.review-detail.commands.reject", icon: 'fa fa-edit',
                executeMethod: rejectReview,
                canExecuteMethod: canReject,
                permission: blade.updatePermission
            },
            {
                name: "customerReviews.blades.review-detail.commands.resetReviewStatus", icon: 'fa fa-edit',
                executeMethod: resetReviewStatus,
                canExecuteMethod: canReset,
                permission: blade.updatePermission
            }
        ];

    }]);