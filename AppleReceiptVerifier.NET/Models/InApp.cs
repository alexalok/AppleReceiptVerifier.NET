using System;

namespace AppleReceiptVerifierNET.Models
{
    /// <summary>
    ///     In-app purchase receipt fields.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/responsebody/receipt/in_app">(docs)</a>
    /// </summary>
    /// <param name="CancellationDate">
    ///     The time Apple customer support canceled a transaction, or the time an auto-renewable subscription plan was
    ///     upgraded. This field is only present for refunded transactions.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/cancellation_date_ms">(docs)</a>
    /// </param>
    /// <param name="CancellationReason">
    ///     The reason for a refunded transaction. When a customer cancels a transaction, the App
    ///     Store gives them a refund and provides a value for this key.
    /// </param>
    /// <param name="ExpiresDate">
    ///     The time a subscription expires or when it will renew.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/expires_date_ms">(docs)</a>
    /// </param>
    /// <param name="IsInIntroOfferPeriod">
    ///     An indicator of whether an auto-renewable subscription is in the introductory price period.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/is_in_intro_offer_period">(docs)</a>
    /// </param>
    /// <param name="IsTrialPeriod">
    ///     An indicator of whether a subscription is in the free trial period.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/is_trial_period">(docs)</a>
    /// </param>
    /// <param name="OriginalPurchaseDate">
    ///     The time of the original app purchase. For an auto-renewable subscription, this value indicates the date of the
    ///     subscription’s initial purchase.
    ///     The original purchase date applies to all product types and remains the same in all transactions for the same
    ///     product ID.
    ///     This value corresponds to the original transaction’s <i>transactionDate</i> property in StoreKit.
    /// </param>
    /// <param name="OriginalTransactionId">
    ///     The transaction identifier of the original purchase.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/original_transaction_id">(docs)</a>
    /// </param>
    /// <param name="ProductId">
    ///     The unique identifier of the product purchased. You provide this value when creating the product in App Store
    ///     Connect,
    ///     and it corresponds to the <i>productIdentifier</i> property of the
    ///     <a href="https://developer.apple.com/documentation/storekit/skpayment">SKPayment</a> object stored in the
    ///     transaction’s payment property.
    /// </param>
    /// <param name="PromotionalOfferId">
    ///     The identifier of the subscription offer redeemed by the user.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/promotional_offer_id">(docs)</a>
    /// </param>
    /// <param name="PurchaseDate">
    ///     For consumable, non-consumable, and non-renewing subscription products, the time the App Store charged the user’s
    ///     account for a purchased or restored product.
    ///     For auto-renewable subscriptions, the time the App Store charged the user’s account for a subscription purchase or
    ///     renewal after a lapse.
    /// </param>
    /// <param name="Quantity">
    ///     The number of consumable products purchased. This value corresponds to the quantity property of the
    ///     <a href="https://developer.apple.com/documentation/storekit/skpayment">SKPayment</a> object stored in the
    ///     transaction’s payment property.
    ///     The value is usually <b>1</b> unless modified with a mutable payment. The maximum value is <b>10</b>.
    /// </param>
    /// <param name="TransactionId">
    ///     A unique identifier for a transaction such as a purchase, restore, or renewal.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/transaction_id">(docs)</a>
    /// </param>
    /// <param name="WebOrderLineItemId">
    ///     A unique identifier for purchase events across devices, including subscription-renewal
    ///     events. This value is the primary key for identifying subscription purchases.
    /// </param>
    /// <param name="InAppOwnershipType">
    ///     A value that indicates whether the user is the purchaser of the product, or is a family member with access to the
    ///     product through Family Sharing.
    ///     <a href="https://developer.apple.com/documentation/appstorereceipts/in_app_ownership_type">(docs)</a>
    /// </param>
    public record InApp(DateTimeOffset? CancellationDate, CancellationReason? CancellationReason,
        DateTimeOffset? ExpiresDate,
        bool IsInIntroOfferPeriod, bool IsTrialPeriod, DateTimeOffset OriginalPurchaseDate, long OriginalTransactionId,
        string ProductId, string? PromotionalOfferId, DateTimeOffset PurchaseDate, int? Quantity,
        long TransactionId, long WebOrderLineItemId, InAppOwnershipType? InAppOwnershipType);
}
