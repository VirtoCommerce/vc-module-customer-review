# Overview

[![CI status](https://github.com/VirtoCommerce/vc-module-customer-review/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-customer-review/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review)
 
The Product Rating and Reviews module allows your customers to add reviews and ratings to products.
 
To manage customer ratings and reviews, moderate content, and collect information before publishing it live, you can use Admin Portal.
 
One can leverage rating information for sorting and filtering products. Product ratings and reviews can be displayed for customers on demand.
 
> ***Note:*** *70% of online shoppers say reviews are a decision maker for a purchase.*
 
## Key Features
The Product Rating and Reviews module enables:
* Submitting product reviews
* Updating the existing product reviews
* Getting products ratings
* Viewing products reviews
* Multi-store capability with every store having its own review for the same product
* Moderating and checking reviews
* Configurable rating calculator, both average and [Wilson](https://www.evanmiller.org/how-not-to-sort-by-average-rating.html)
* Email review reminder for customers who purchased products prompting them to come back and leave a review
 
## Current Constraints
* Product reviews are linked to the store
* One review per one customer, per product, and per store
 
 
## Scenarios

> ***Note:*** *In order to get access to some scenarios, you need to integrate the Product Rating and Reviews API into your frontend application.
 
### Submitting Product Review
Customers usually submit product reviews to provide feedback and improve the decision making process for the purchase.
 
You can submit a product review from the front end application by calling an API like this:
 
```json
POST /api/customerReviews
 
[
  {
    "storeId": "B2B-store",
    "userId": "test_user_id",
    "userName": "Alex B.",
    "productId": "baa4931161214690ad51c50787b1ed94",
    "title": "Demo Product Review",
    "review": "Nice Product. I liked it",
    "rating": 5
  }
]
```
 
 
### Getting Products Ratings

To view product rating in the Admin Portal, go to *Catalog* and open the *Product* screen:
 
![View Product Reviews in the Admin Porta](media/view-product-reviews.png)
 
From the front end application, you can call an API and request current rating for multiple products and a specific store:
 
```json
POST /api/rating/productRatingInStore
 
{
  "storeId": "B2B-store",
  "productIds": [
    "baa4931161214690ad51c50787b1ed94"
  ]
}
 
Response:
[
  {
    "productId": "baa4931161214690ad51c50787b1ed94",
    "value": 4.5,
    "reviewCount": 4
  }
]
```
 
 
### Viewing Product Reviews
Just like product rating, you can view product reviews in the Admin Portal by navigating to *Catalog > Product*:
 
![View Product Reviews in the Admin Porta](media/view-product-reviews.png)
 
 
From the front end app, you can call an API and request current rating for multiple products and a specific store:

```json
POST /api/customerReviews/reviewList
 
{
    "productIds":["baa4931161214690ad51c50787b1ed94"],
    "storeId":"B2B-store",
    "reviewStatus":[1],
    "sort":"",
    "take":20,
    "skip":0,
}
 
Response:
{
    "totalCount":1,
    "results":[
        {
            "id":"fc9a07db-f09f-4a85-bafc-f5e9299d3301",
            "productName":"1\" Stainless Steel Carriage Bolt, 18-8, NL-19(SM) Finish, 1/4\"-20 Dia/Thread Size, 50 PK",
            "reviewStatus":"Approved",
            "reviewStatusId":1,
            "title": "Demo Product Review",
            "review": "Nice Product. I liked it",
            "rating":5,
            "userName":"Alex B.",
            "storeName":"B2B-store",
            "createdDate":"2022-01-31T14:09:15.491305Z"
        }
    ]
}
```
 
 
### Moderating Reviews
Moderating product reviews is crucial, as it allows you to remove undesired content and protect your web store from spam.
 
> ***Note:*** The moderation process can be customized based on the solution requirements.
 
Before changing the review status, you can first read its contents. To read a review, select *Product Rating and Reviews*:
 
![view new reviews](media/view-review-list.png)

You can also apply filtering and sorting, if required.

To change the status of a customer review, all you need to do is:
1. Open the review in question and
1. In the toolbar, click *Approve Review* or *Reject Review*:

![view new reviews](media/moderate-customer-review.png)
 
You cannot remove reviews through the Admin Portal; however, this option is available thorugh the following API you can call from your front end app:
 
```json
POST 
​/api​/customerReviews​/approve
 
[
    "fc9a07db-f09f-4a85-bafc-f5e9299d3301"
]
```
 
```json
POST 
/api/customerReviews/reject
 
[
    "fc9a07db-f09f-4a85-bafc-f5e9299d3301"
]
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
* Enable product reviews (default value: false)
* Allow anonymous product reviews (default value: false)
* Allow reviews only from customers who made a purchase (default value: true)
 
### Email Review Reminder Settings
* Enable sending a review email reminder to a customer when an order has a particular status for a specified number of days (default value: false)
* Cron expression for processing the email sending job (default value: every 15 minutes)
* Number of days for a particular order status (default value: 10 days)
* Order status that enables sending an email (default value: Complete)
* Maximum emails per customer (default value: 2)
