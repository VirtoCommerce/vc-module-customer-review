# Rest API

!!! attention
    This is an auto-generated document. Used [swagger-markdown](https://github.com/syroegkin/swagger-markdown).

## Endpoints

### /api/customerReviews/reviewList

#### POST
##### Summary

Return list of reviews with product and store name

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:read |

### /api/customerReviews/search

#### POST
##### Summary

Return product Customer review search results

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:read |

### /api/customerReviews/changes

#### POST
##### Summary

Return productIds from changed reviews

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/customerReviews/approve

#### POST
##### Summary

Accept existing customer review

##### Responses

| Code | Description |
| ---- | ----------- |
| 204 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:update |

### /api/customerReviews/reject

#### POST
##### Summary

Reject existing customer review

##### Responses

| Code | Description |
| ---- | ----------- |
| 204 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:update |

### /api/customerReviews/reset

#### POST
##### Summary

Set New existing customer review

##### Responses

| Code | Description |
| ---- | ----------- |
| 204 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:update |

### /api/customerReviews

#### POST
##### Summary

Create new or update existing customer review

##### Responses

| Code | Description |
| ---- | ----------- |
| 204 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:update |

#### DELETE
##### Summary

Delete Customer Reviews by IDs

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| ids | query | IDs | No | [ string ] |

##### Responses

| Code | Description |
| ---- | ----------- |
| 204 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:delete |

### /api/customerReviews/viewedRequestReview

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 204 | Success |

### /api/rating/productRatingInCatalog

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:ratingRead |

### /api/rating/productRatingInStore

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:ratingRead |

### /api/rating/calculateStore

#### POST
##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| storeId | query |  | No | string |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 401 | Unauthorized |
| 403 | Forbidden |

##### Security

| Security Schema | Scopes |
| --- | --- |
| oauth2 | customerReviews:ratingRecalc |

### Models

#### SortDirection

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| SortDirection | string |  |  |

#### SortInfo

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| sortColumn | string |  | No |
| sortDirection | string | _Enum:_ `"Ascending"`, `"Descending"` | No |

#### CustomerReviewSearchCriteria

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| productIds | [ string ] |  | No |
| reviewStatus | [ integer ] |  | No |
| storeId | string |  | No |
| modifiedDate | dateTime |  | No |
| userId | string |  | No |
| responseGroup | string |  | No |
| objectType | string |  | No |
| objectTypes | [ string ] |  | No |
| objectIds | [ string ] |  | No |
| keyword | string |  | No |
| searchPhrase | string |  | No |
| languageCode | string |  | No |
| sort | string |  | No |
| sortInfos | [ object ] |  | No |
| skip | integer |  | No |
| take | integer |  | No |

#### CustomerReviewListItem

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| id | string |  | No |
| productName | string |  | No |
| reviewStatus | string |  | No |
| reviewStatusId | integer |  | No |
| title | string |  | No |
| review | string |  | No |
| rating | integer |  | No |
| userName | string |  | No |
| storeName | string |  | No |
| createdDate | dateTime |  | No |

#### CustomerReviewListItemSearchResult

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| totalCount | integer |  | No |
| results | [ object ] |  | No |

#### CustomerReviewStatus

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| CustomerReviewStatus | string |  |  |

#### CustomerReview

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| title | string |  | No |
| review | string |  | No |
| rating | integer |  | No |
| userId | string |  | No |
| userName | string |  | No |
| productId | string |  | No |
| storeId | string |  | No |
| reviewStatus | string | _Enum:_ `"New"`, `"Approved"`, `"Rejected"` | No |
| createdDate | dateTime |  | No |
| modifiedDate | dateTime |  | No |
| createdBy | string |  | No |
| modifiedBy | string |  | No |
| id | string |  | No |

#### CustomerReviewSearchResult

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| totalCount | integer |  | No |
| results | [ object ] |  | No |

#### ChangedReviewsQuery

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| modifiedDate | dateTime |  | No |

#### ProductCatalogRatingRequest

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| catalogId | string |  | No |
| productIds | [ string ] |  | No |

#### RatingStoreDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| storeName | string |  | No |
| storeId | string |  | No |
| productId | string |  | No |
| value | double |  | No |
| reviewCount | integer |  | No |

#### ProductStoreRatingRequest

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| storeId | string |  | No |
| productIds | [ string ] |  | No |

#### RatingProductDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| productId | string |  | No |
| value | double |  | No |
| reviewCount | integer |  | No | 
