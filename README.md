# Virto Commerce Rating and Reviews Module

[![CI status](https://github.com/VirtoCommerce/vc-module-customer-review/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-customer-review/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-customer-review&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-customer-review)

## Overview

The Rating and Reviews (Customer Reviews) module enables shoppers to submit reviews and ratings for products and other entities in a Virto Commerce store. Merchandisers and moderators can review, approve, reject, or delete content through the Admin Portal before it goes live, while customer-facing applications can consume ratings and reviews through REST or GraphQL APIs for listing, sorting, and filtering.

The module is built on top of a flexible `EntityId` / `EntityType` contract, which means reviews are not limited to products — the module can be extended to rate vendors, delivery services, orders, managers, price lists, or any other domain object used in your solution.

> ***Note:*** *70% of online shoppers say reviews are a decision maker for a purchase.*

## Key features

* **Review lifecycle management** — create, update, moderate (approve/reject/reset), and delete reviews through the Admin Portal or REST API.
* **Rich review content** — title, body, numeric rating, and up to N attached images per review (configurable).
* **Rating aggregation** — pluggable rating calculators with two built-in strategies: `Average` and [Wilson lower bound](https://www.evanmiller.org/how-not-to-sort-by-average-rating.html). Ratings are computed per entity and per store.
* **Multi-store and store-agnostic support** — each store maintains its own ratings and can be configured independently; non-store-scoped reviews are also supported.
* **Flexible entity model** — reviews are keyed by `EntityId` + `EntityType`, so additional object types (vendor, order, custom objects) can be registered without modifying the module's source code.
* **Anonymous vs. authenticated reviews** — per-store toggle to allow or require a signed-in user.
* **"Purchase-verified" reviews** — optional store-level rule restricting reviews to customers who have previously ordered the product.
* **Email review reminders** — recurring Hangfire job that emails buyers after their order reaches a configured status (e.g. `Completed`), prompting them to leave feedback. Reminder cadence, eligibility window, and the maximum number of reminders per customer are all configurable.
* **GraphQL (xAPI) integration** — storefront-facing GraphQL schema for querying reviews, checking whether the current user can leave feedback, and submitting new reviews.
* **Image uploads** — integrates with `VirtoCommerce.FileExperienceApi` / Assets for a dedicated `review-images` scope.
* **Event-driven integrations** — publishes `CustomerReviewChangeEvent`, `CustomerReviewChangedEvent`, and `ReviewStatusChangedEvent` so other modules can react to moderation changes and rating recalculations.
* **Multi-database support** — ships with EF Core providers for SQL Server, PostgreSQL, and MySQL.
* **Permissions-aware Admin UI** — fine-grained permissions for reading reviews, updating, deleting, reading ratings, and triggering rating recalculation.

## Documentation

* [Rating and Reviews module user documentation](https://docs.virtocommerce.org/platform/user-guide/rating-reviews/overview/)
* [GraphQL API documentation](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/Reviews/overview/)
* [REST API](https://virtostart-demo-admin.govirto.com/docs/index.html?urls.primaryName=VirtoCommerce.CustomerReviews)
* [Reviewing products on Frontend](https://docs.virtocommerce.org/storefront/user-guide/account/review-products)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-customer-review)

## Configuration

All settings are registered under the `VirtoCommerce.CustomerReviews` module and can be managed from the Admin Portal (*Settings*) or via the Platform settings API. Settings are split across three scopes: platform-wide, per-store, and the email-reminder background job.

### Store settings (*Store → Product Reviews*)

| Setting | Description | Default |
| --- | --- | --- |
| `CustomerReviews.CustomerReviewsEnabled` | Enables the review feature for the store. | `false` |
| `CustomerReviews.CustomerReviewsEnabledForAnonymous` | Allows anonymous (unauthenticated) users to submit reviews. | `false` |
| `CustomerReviews.CanSubmitReviewWhenHasOrder` | Restricts review submission to customers who have an order containing the product. | `true` |
| `CustomerReviews.Calculation.Method` | Rating aggregation strategy. Allowed values are populated from registered `IRatingCalculator` implementations (`Average`, `Wilson`). | `Average` |

### General product-review settings (*Product Reviews*)

| Setting | Description | Default |
| --- | --- | --- |
| `CustomerReviews.ReviewMaximumImages` | Maximum number of images a user can attach to a single review. | `5` |

### Email Review Reminder (*Product Reviews → Email Review Reminder*)

| Setting | Description | Default |
| --- | --- | --- |
| `CustomerReviews.CustomerReviewsEnabledRequestReviewJob` | Master switch for the review-reminder background job. | `false` |
| `CustomerReviews.CustomerReviewsRequestReviewCronJob` | Cron expression for the reminder job schedule. | `0/15 * * * *` (every 15 minutes) |
| `CustomerReviews.CustomerReviewsRequestReviewDaysInState` | Number of days the order must stay in the configured status before a reminder is sent. | `10` |
| `CustomerReviews.CustomerReviewsRequestReviewOrderInState` | Order status that triggers the reminder eligibility window. Allowed values are sourced from the Orders module. | `Completed` |
| `CustomerReviews.CustomerReviewsRequestReviewMaxRequests` | Maximum number of reminder emails sent to a single customer per order. | `2` |

### Permissions

The module registers the following permissions (under the `CustomerReviews` group):

* `customerReviews:read`
* `customerReviews:update`
* `customerReviews:delete`
* `customerReviews:ratingRead`
* `customerReviews:ratingRecalc`

### Notifications

Registers the `CustomerReviewEmailNotification` template, which can be customized globally (*Notifications → Notification list*) or per store (*Store → Notifications*) under the name *Order request review notification*.

## Architecture

The solution is organized as a classic Virto Commerce module with clear separation between contracts, implementation, persistence, Web/Admin host, and xAPI.

```
src/
├── VirtoCommerce.CustomerReviews.Core              # Domain contracts: models, service interfaces, events, notifications, ModuleConstants
├── VirtoCommerce.CustomerReviews.Data              # EF Core entities, repositories, services, background jobs, event handlers
├── VirtoCommerce.CustomerReviews.Data.SqlServer    # SQL Server provider + migrations
├── VirtoCommerce.CustomerReviews.Data.PostgreSql   # PostgreSQL provider + migrations
├── VirtoCommerce.CustomerReviews.Data.MySql        # MySQL provider + migrations
├── VirtoCommerce.CustomerReviews.ExperienceApi     # GraphQL (xAPI) schema: queries, commands, types, validators, authorization
└── VirtoCommerce.CustomerReviews.Web               # Module host: Module.cs, REST controllers, Admin UI (AngularJS), manifest
tests/
└── VirtoCommerce.CustomerReviews.Test              # Unit tests
```

### Core domain model

* `CustomerReview` — auditable entity with `Title`, `Review`, `Rating`, `UserId`, `UserName`, `EntityId`, `EntityType`, `EntityName`, `StoreId`, `ReviewStatus`, and a collection of `CustomerReviewImage`.
* `CustomerReviewStatus` — moderation state (`New`, `Approved`, `Rejected`, …).
* `RatingEntityDto` / `RatingStoreDto` / `RatingProductDto` — aggregated rating projections consumed by the REST and GraphQL APIs.
* `ReviewEntityTypes.Product` — built-in entity type; custom types can be registered client-side without changing the module.

### Services (Core → Data)

| Contract | Implementation | Responsibility |
| --- | --- | --- |
| `ICustomerReviewService` | `CustomerReviewService` | CRUD for reviews, moderation transitions, domain event emission. |
| `ICustomerReviewSearchService` | `CustomerReviewSearchService` | Paged/filterable search by entity, store, status, text. |
| `IRatingService` | `RatingService` | Aggregated rating reads and recalculation per entity/store. |
| `IRatingCalculator` | `AverageRatingCalculator`, `WilsonRatingCalculator` | Pluggable rating aggregation; the active strategy is chosen via the `Calculation.Method` setting. |
| `IRequestReviewService` | `RequestReviewService` | Identifies orders eligible for a review reminder and issues notifications. |

### Background jobs and event handlers

* `RequestCustomerReviewJob` — Hangfire recurring job wired through `IRecurringJobService.WatchJobSetting`. It turns on/off based on `RequestReviewEnableJob` and re-registers when `RequestReviewCronJob` changes.
* `ReviewStatusChangedEventHandler` — reacts to review status transitions (e.g. to recalculate ratings).
* `OrderChangedEventHandler` — listens to `OrderChangedEvent` from the Orders module to drive the reminder eligibility logic.

### REST API surface

Hosted by `VirtoCommerce.CustomerReviews.Web`:

* `CustomerReviewsModuleController` — review CRUD, search, approve/reject/reset, bulk delete.
* `CustomerReviewsModuleRatingController` — entity rating queries and recalculation.

### GraphQL (xAPI) surface

Hosted by `VirtoCommerce.CustomerReviews.ExperienceApi` under the `customerReviews` schema:

* **Queries** — `CustomerReviewsQuery` (paged/filtered reviews), `CanLeaveFeedbackQuery` (per-user eligibility check).
* **Mutations** — `CreateReviewCommand` (submit a review, subject to authorization and validators).
* **Schemas / types** — `CustomerReviewType`, `CustomerReviewImageType`, `CustomerReviewStatusType`, `CreateReviewCommandType`, `CreateReviewResultType`, `ReviewValidationErrorType`.
* **Authorization / validation** — dedicated `Authorization` and `Validators` folders enforce identity and input rules before the command handler runs.

## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-customer-review/releases/latest)

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
