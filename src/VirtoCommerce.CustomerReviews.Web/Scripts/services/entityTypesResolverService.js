angular.module('VirtoCommerce.CustomerReviews')
    .factory('VirtoCommerce.CustomerReviews.entityTypesResolverService', function () {
        return {
            objects: [],
            registerType: function (entityTypeDefinition) {
                entityTypeDefinition.detailBlade = angular.extend({
                    id: "entityDetail",
                    metaFields: [],
                    entityTypeDefinition: entityTypeDefinition
                }, entityTypeDefinition.detailBlade);

                entityTypeDefinition.knownChildrenTypes = entityTypeDefinition.knownChildrenTypes || [];

                this.objects.push(entityTypeDefinition);
            },
            resolve: function (type) {
                return _.findWhere(this.objects, { entityType: type });
            }
        };
    });
