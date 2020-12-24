using System;

namespace AppleReceiptVerifierNET.Models
{
    /// <summary>
    /// In-app purchase transaction. <a href="https://developer.apple.com/documentation/appstorereceipts/responsebody/latest_receipt_info">(docs)</a>
    /// </summary>
    /// <param name="CancellationDate">The time Apple customer support canceled a transaction, or the time an auto-renewable subscription plan was upgraded. This field is only present for refunded transactions.</param>
    /// <param name="CancellationReason">The reason for a refunded transaction. When a customer cancels a transaction, the App Store gives them a refund and provides a value for this key.</param>
    /// <param name="ExpiresDate">The time a subscription expires or when it will renew.</param>
    /// <param name="InAppOwnershipType">A value that indicates whether the user is the purchaser of the product, or is a family member with access to the product through Family Sharing. 
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/in_app_ownership_type">(docs)</a></param>
    /// <param name="IsInIntroOfferPeriod">An indicator of whether an auto-renewable subscription is in the introductory price period. 
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/is_in_intro_offer_period">(docs)</a></param>
    /// <param name="IsTrialPeriod">An indicator of whether a subscription is in the free trial period. <a href="https://developer.apple.com/documentation/appstorereceipts/is_trial_period">(docs)</a></param>
    /// <param name="IsUpgraded">An indicator that a subscription has been canceled due to an upgrade. This field is only present for upgrade transactions.</param>
    /// <param name="OfferCodeRefName">The reference name of a subscription offer that you configured in App Store Connect. This field is present when a customer redeemed a subscription offer code.
    /// For more information about offer codes, see <a href="https://help.apple.com/app-store-connect/#/dev6a098e4b1">Set Up Offer Codes</a>, 
    /// and <a href="https://developer.apple.com/documentation/storekit/in-app_purchase/subscriptions_and_offers/implementing_offer_codes_in_your_app">Implementing Offer Codes in Your App.</a></param>
    /// <param name="OriginalPurchaseDate">The time of the original app purchase. For an auto-renewable subscription, this value indicates the date of the subscription’s initial purchase. 
    /// The original purchase date applies to all product types and remains the same in all transactions for the same product ID. 
    /// This value corresponds to the original transaction’s <i>transactionDate</i> property in StoreKit.</param>
    /// <param name="OriginalTransactionId">The transaction identifier of the original purchase. <a href="https://developer.apple.com/documentation/appstorereceipts/original_transaction_id">(docs)</a></param>
    /// <param name="ProductId">The unique identifier of the product purchased. You provide this value when creating the product in App Store Connect, 
    /// and it corresponds to the <i>productIdentifier</i> property of the <a href="https://developer.apple.com/documentation/storekit/skpayment">SKPayment</a> object stored in the transaction’s payment property.</param>
    /// <param name="PromotionalOfferId">The identifier of the subscription offer redeemed by the user. <a href="https://developer.apple.com/documentation/appstorereceipts/promotional_offer_id">(docs)</a></param>
    /// <param name="PurchaseDate">For consumable, non-consumable, and non-renewing subscription products, the time the App Store charged the user’s account for a purchased or restored product. 
    /// For auto-renewable subscriptions, the time the App Store charged the user’s account for a subscription purchase or renewal after a lapse.</param>
    /// <param name="Quantity">The number of consumable products purchased. This value corresponds to the quantity property of the <a href="https://developer.apple.com/documentation/storekit/skpayment">SKPayment</a> object stored in the transaction’s payment property. 
    /// The value is usually <b>1</b> unless modified with a mutable payment. The maximum value is <b>10</b>.</param>
    /// <param name="SubscriptionGroupIdentifier">The identifier of the subscription group to which the subscription belongs. 
    /// The value for this field is identical to the <a href="https://developer.apple.com/documentation/storekit/skproduct/2981047-subscriptiongroupidentifier">subscriptionGroupIdentifier</a> property in SKProduct.</param>
    /// <param name="WebOrderLineItemId">A unique identifier for purchase events across devices, including subscription-renewal events. This value is the primary key for identifying subscription purchases.</param>
    /// <param name="TransactionId">A unique identifier for a transaction such as a purchase, restore, or renewal. <a href="https://developer.apple.com/documentation/appstorereceipts/transaction_id">(docs)</a></param>
    public record ReceiptInfo(DateTimeOffset? CancellationDate, CancellationReason? CancellationReason, DateTimeOffset ExpiresDate, InAppOwnershipType? InAppOwnershipType,
        bool IsInIntroOfferPeriod, bool IsTrialPeriod, bool? IsUpgraded, string? OfferCodeRefName,
        DateTimeOffset OriginalPurchaseDate, long OriginalTransactionId, string ProductId, string? PromotionalOfferId,
        DateTimeOffset PurchaseDate, int? Quantity, int SubscriptionGroupIdentifier, long WebOrderLineItemId, long TransactionId);

    /// <summary>
    /// The reason for a refunded transaction.
    /// </summary>
    public enum CancellationReason
    {
        /// <summary>
        /// The transaction was canceled for the reason other than an actual or perceived issue within your app; 
        /// for example, if the customer made the purchase accidentally.
        /// </summary>
        Other,

        /// <summary>
        /// The customer canceled their transaction due to an actual or perceived issue within your app.
        /// </summary>
        IssueWithinApp
    }

    /// <summary>
    /// The relationship of the user with the family-shared purchase to which they have access. <a href="https://developer.apple.com/documentation/appstorereceipts/in_app_ownership_type">(docs)</a>
    /// </summary>
    public enum InAppOwnershipType
    {
        /// <summary>
        /// The transaction belongs to a family member who benefits from service.
        /// </summary>
        FamilyShared,

        /// <summary>
        /// The transaction belongs to the purchaser.
        /// </summary>
        Purchased
    }
}