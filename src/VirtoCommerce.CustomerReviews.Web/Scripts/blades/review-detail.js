angular.module('VirtoCommerce.CustomerReviews')
    .controller('VirtoCommerce.CustomerReviews.reviewDetailController',
        ['$scope', 'platformWebApp.bladeNavigationService', 'CustomerReviews.WebApi',
            'VirtoCommerce.CustomerReviews.entityTypesResolverService', 'platformWebApp.dialogService',
            'VirtoCommerce.RatingApi',
            function ($scope, bladeNavigationService, reviewsApi, entityTypesResolverService, dialogService, ratingApi) {
        var blade = $scope.blade;
        blade.updatePermission = 'customerReviews:update';

        blade.refresh = function () {
            blade.origEntity = blade.currentEntity;
            blade.securityScopes = blade.currentEntity.securityScopes;
            blade.currentEntity = angular.copy(blade.currentEntity);
            getEntityObject(blade.currentEntity.entityId);
            getEntityRating(blade.currentEntity.entityId, blade.currentEntity.entityType);
            blade.isLoading = false;
        }

        blade.ratingValidator = function (value) {
            var pattern = new RegExp("^([1-5])$");
            return pattern.test(value);
        };

        blade.formScope = null;
        $scope.setForm = function (form) { blade.formScope = form; };

        blade.openReviewEntityDetails = function() {
            var foundTemplate = entityTypesResolverService.resolve(blade.currentEntity.entityType);
            if (foundTemplate) {
                var newBlade = angular.copy(foundTemplate.detailBlade);
                newBlade[foundTemplate.entityIdFieldName] = blade.currentEntity.entityId;
                bladeNavigationService.showBlade(newBlade, blade);
            } else {
                dialogService.showNotificationDialog({
                    id: "error",
                    title: "customerReviews.dialogs.unknown-entity-type.title",
                    message: "customerReviews.dialogs.unknown-entity-type.message",
                    messageValues: { entityType: blade.currentEntity.entityType }
                });
            }
        }

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity);
        };

        function canSave() {
            return isDirty() && blade.formScope && blade.formScope.$valid;
        }

        function saveChanges() {
            blade.isLoading = true;
            reviewsApi.update({}, [blade.currentEntity],
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

        function getEntityObject(entityId) {
            var foundTemplate = entityTypesResolverService.resolve(blade.currentEntity.entityType);
            if (foundTemplate && foundTemplate.getEntity) {
                return foundTemplate.getEntity(entityId, setEntityObjectCallback);
            }
            return {};
        };

        function setEntityObjectCallback(entityName, entityImage) {
            blade.entityObject = { name: entityName, image: entityImage };
        }

        function getEntityRating(entityId, entityType) {
            ratingApi.get({ entityIds: [entityId], entityType: entityType }, function (data) {
                const ratingValues = data.map(x => x.value);
                if (ratingValues.length > 0) {
                    blade.ratingValue = ratingValues[0].toFixed(1) + '/5';
                }
                return 0;
            });
        }

        function approveReview() {
            reviewsApi.approve({}, [blade.currentEntity.id],
                function (data, headers) {
                    blade.isLoading = false;
                    blade.parentBlade.refresh(true);
                    $scope.bladeClose();
                },
                function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                    blade.isLoading = false;
                });
        }

        function rejectReview() {
            reviewsApi.reject({}, [blade.currentEntity.id],
                function (data, headers) {
                    blade.isLoading = false;
                    blade.parentBlade.refresh(true);
                    $scope.bladeClose();
                },
                function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                    blade.isLoading = false;
                });
        }

        function resetReviewStatus() {
            reviewsApi.resetReviewStatus({}, [blade.currentEntity.id],
                function (data, headers) {
                    blade.isLoading = false;
                    blade.parentBlade.refresh(true);
                    $scope.bladeClose();
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

        function deleteReview() {
            var dialog = {
                id: "confirmDeleteReview",
                title: "customerReviews.dialogs.review-delete.title",
                message: "customerReviews.dialogs.review-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        reviewsApi.delete({ ids: [blade.currentEntity.id] }, function () {
                            $scope.bladeClose();
                            blade.parentBlade.refresh();
                        });
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        }

        blade.toolbarCommands = [
            {
                name: "customerReviews.blades.review-detail.commands.approve", icon: 'fa fa-cloud-upload',
                executeMethod: approveReview,
                canExecuteMethod: canApprove,
                permission: blade.updatePermission
            },
            {
                name: "customerReviews.blades.review-detail.commands.reject", icon: 'fa fa-unlink',
                executeMethod: rejectReview,
                canExecuteMethod: canReject,
                permission: blade.updatePermission
            },
            {
                name: "customerReviews.blades.review-detail.commands.resetReviewStatus", icon: 'fa fa-edit',
                executeMethod: resetReviewStatus,
                canExecuteMethod: canReset,
                permission: blade.updatePermission
            },
            {
                name: "platform.commands.delete", icon: 'fas fa-trash-alt',
                executeMethod: deleteReview,
                canExecuteMethod: function () {
                    return true;
                }
            }
        ];

        blade.refresh();

    }]);
