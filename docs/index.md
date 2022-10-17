## Overview

[![CI status](https://github.com/VirtoCommerce/vc-module-customer-review/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-customer-review/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review)
 
The Rating and Reviews module allows your users to add reviews and ratings for almost any type of object in the system, such as Products, Vendors, Delivery Services, Orders, you can even create reviews for objects such as Managers, Customers, Price Lists, anything that may require rating when building your business applications.

To manage user ratings and reviews, moderate content, and collect information before publishing it, you can use Admin Portal.
 
One can leverage rating information for sorting and filtering review objects. Ratings and reviews can be displayed for users on demand.
 
> ***Note:*** *70% of online shoppers say reviews are a decision maker for a purchase.*

## Key features
The Rating and Reviews module enables:

* Moderating and checking reviews
* Approving or rejecting reviews
* Updating the existing reviews
* Getting ratings
* Multi-store and none-store capability with every store having its own review
* Configurable rating calculator, both average and [Wilson](https://www.evanmiller.org/how-not-to-sort-by-average-rating.html)
* Email review reminder for customers who purchased products prompting them to come back and leave a review

## Ratings and Reviews for custom object types

The flexibility of use contains in the structure of Review - when storing and querying, the EntityId and EntityType fields are used, which allow you to describe almost any object used in the application. The ReviewsModule allows developers to define the required object types themselves, in their production systems, without directly modifying the ReviewsModule source code. When registering the type of an object being viewed, the developer specifies the Templates and Controllers that will handle the rendering of the object.

As an example, you can use the described Product type, which is used for reviews of catalog products (this code is placed in the ReviewsModule as a sample for use, further it is assumed that developers will register Custom Types directly in their Modules). 

`VirtoCommerce.CustomerReviews.js`
```js
55    //Product entityType resolver
56    entityTypesResolverService.registerType({
57        entityType: 'Product',
58        description: 'customerReviews.blades.product-detail.description',
59        fullTypeName: 'VirtoCommerce.CatalogModule.Core.Model.CatalogProduct',
60        icon: 'fas fa-shopping-bag',
61        entityIdFieldName: 'itemId',
62        detailBlade: {
63            controller: 'virtoCommerce.catalogModule.itemDetailController',
64            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html'
65        },
66        getEntity: function (entityId, setEntityCallback) {
67            items.get({ id: entityId, respGroup: 1 }, (data) => {
68                setEntityCallback(data.name, data.imgSrc);
69            });
70        },
71        knownChildrenTypes: []
72    });
```
***Notes:***

Line 57: this entityType will use to resolve objects anywhere - filters, queries, commands.

Lines 63 and 64: controller and template to show object in Admin Portal, you can use "default" object's blades from your Modules. In this example was reused template and controller for "Item" blade from *Catalog*.

Line 66: function to get information about object. In example - method from *Catalog* for getting Product's info

## Scenarios

> ***Note:*** In order to get access to some scenarios, you need to integrate the Rating and Reviews API into your frontend application.
 
### Submitting Review

Users usually submit reviews to provide feedback (for example, about product, or vendor, or delivery service) and improve the decision making process for the purchase.
 
You can submit a review from the front end application by calling an API like this:
 
```json
POST /api/customerReviews
 
[
  {
    "storeId": "Electronics",
    "userId": "test_user_id",
    "userName": "Alex B.",
    "EntityId": "baa4931161214690ad51c50787b1ed94",
    "EntityType": "Product",
    "EntityName": "Aunkler's Favorite Product",
    "title": "Demo Product Review",
    "review": "Nice Product. I liked it",
    "rating": 5
  }
]
```
 
 
### Viewing Ratings and Reviews

To view reviews in the Admin Portal, go to *Rating and Reviews* in main menu:
 
![View Reviews in the Admin Portal](media/view-review-list.png)
 
From the front end application, you can call an API and request list of reviews:
 
```json
POST /api/customerReviews/reviewList
 
{
    "reviewStatus":null,
    "sort":"",
    "take":20,
    "skip":0
}
```
 
Response:
```json
{
    "totalCount":7,
    "results":[
        {
            "id":"fc9a07db-f09f-4a85-bafc-f5e9299d3301",
            "entityId":"8c01bcf0-4bab-4675-97b9-ea6cc41912e5",
            "entityName":"Aunkler's Favorite Product",
            "entityType":"Product",
            "reviewStatus":"Approved",
            "reviewStatusId":1,
            "title": "Demo Product Review",
            "review": "Nice Product. I liked it",
            "rating":5,
            "userName":"Alex B.",
            "storeName":"Electronics",
            "createdDate":"2022-10-04T03:46:43.433Z"
        },
        {...}
    ]
}
```

You can filter list of reviews using additional fields in request (for example):

```json
{
    "entityType":"Product",
    "searchPhrase":"demo",
    "sort":"createdDate:desc",
    "take":20,
    "skip":0,
}
```

### Getting current average Rating of reviewable object

To view entity's rating, as example, for Product, in the Admin Portal, go to *Catalog* in main menu, and open your product's screen. In widjets you can see average rating and product's reviews count. Click to the widget and you can see list of this product's reviews:

![Product's average rating in the Admin Portal](media/view-product-reviews.png)

From the front end application, you can call an API to request rating:

```json
POST /api/rating/entityRating
 
{
    "entityIds": [
        "8c01bcf0-4bab-4675-97b9-ea6cc41912e5"
    ],
    "entityType":"Product"
}
```
 
Response:
```json
[
    {
        "storeName":"Electronics",
        "storeId":"Electronics",
        "entityId":"8c01bcf0-4bab-4675-97b9-ea6cc41912e5",
        "entityType":"Product",
        "value":3.50,
        "reviewCount":2
    }
]
```
 
### Moderating Reviews
Moderating reviews is crucial, as it allows you to remove undesired content and protect your web store from spam.
 
> ***Note:*** The moderation process can be customized based on the solution requirements.
 
Before changing the review status, you can first read its contents. To read a review, select *Rating and Reviews* as described above and filter rewiew status "New". You can also apply extra filtering and sorting, if required.

To change the status of a customer review click to review in list (this will show review's details), and in the toolbar, click *Approve Review* or *Reject Review*:

![Moderate new review](media/moderate-customer-review.png)
 

```json
POST /api/customerReviews/approve
 
[
    "fc9a07db-f09f-4a85-bafc-f5e9299d3301"
]
```
 
```json
POST /api/customerReviews/reject
 
[
    "fc9a07db-f09f-4a85-bafc-f5e9299d3301"
]
```

You can rollback approved or rejected modetated review's status - just use "Reset review status" option in toolbar. This will recalculate entity's rating excluding current review.

```json
POST /api/customerReviews/reset
 
[
    "fc9a07db-f09f-4a85-bafc-f5e9299d3301"
]
```

You can remove unwanted reviews (like a spam) from your system.

```json
DELETE /api/customerReviews

{
    "ids":
    [
        "fc9a07db-f09f-4a85-bafc-f5e9299d3301"
    ]
} 
```
 
### Email Review Reminder
There is also an option to configure email review reminder for the customers who purchased products but still did not leave any feedback.
 
Before enabling the Email Review Reminder, you need to configure and/or customize the email template:
 
![Email Review Reminder Notification Template](media/order-request-review-notification.png)
 
To customize the Email Review Reminder template, navigate to: *Notifications > Notification list > Order request review notification*.
 
You can also customize the Email Review Reminder template for the store. To do so, navigate to *Store > Notifications > Notification list> Order request review notification*. 
 
Once you have configured your template, you can activate the *Email Review Reminder* option in *Settings > Product Review > Email Review Reminder*. By default, this feature is disabled.
 
![Email Reminder Settings](media/email-reminder-settings.png)
 
 
## Settings

Currently, there are three types of settings you can configure within the Product Rating and Reviews module.

### General Settings
* Rating calculation method: Enables selecting rating method, average or [Wilson](https://www.evanmiller.org/how-not-to-sort-by-average-rating.html)
 
### Store Settings
* Enable reviews (default value: false)
* Allow anonymous reviews (default value: false)
* Allow reviews only from customers who made a purchase (default value: true)
 
### Email Review Reminder Settings
* Enable sending a review email reminder to a customer when an order has a particular status for a specified number of days (default value: false)
* Cron expression for processing the email sending job (default value: every 15 minutes)
* Number of days for a particular order status (default value: 10 days)
* Order status that enables sending an email (default value: Complete)
* Maximum emails per customer (default value: 2)
