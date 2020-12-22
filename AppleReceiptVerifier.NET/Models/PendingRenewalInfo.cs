using System;

namespace AppleReceiptVerifier.NET.Models
{
    /// <summary>
    /// Auto-renewable subscription renewal that is open or failed in the past. <a href="https://developer.apple.com/documentation/appstorereceipts/responsebody/pending_renewal_info">(docs)</a>
    /// </summary>
    /// <param name="AutoRenewProductId">The current renewal preference for the auto-renewable subscription. The value for this key corresponds to the <i>productIdentifier</i> property of the product that the customer’s subscription renews. 
    /// This field is only present if the user downgrades or crossgrades to a subscription of a different duration for the subsequent subscription period.</param>
    /// <param name="AutoRenewStatus">The renewal status for the auto-renewable subscription. <a href="https://developer.apple.com/documentation/appstorereceipts/auto_renew_status">(docs)</a></param>
    /// <param name="ExpirationIntent">The reason a subscription expired. This field is only present for a receipt that contains an expired auto-renewable subscription.
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/expiration_intent">(docs)</a></param>
    /// <param name="GracePeriodExpiresDate">The time at which the grace period for subscription renewals expires. 
    /// This key is only present for apps that have Billing Grace Period enabled and when the user experiences a billing error at the time of renewal. </param>
    /// <param name="IsInBillingRetryPeriod">A flag that indicates Apple is attempting to renew an expired subscription automatically. 
    /// This field is only present if an auto-renewable subscription is in the billing retry state.
    /// <b>true</b> if the App Store is attempting to renew the subscriptio <b>false</b> if the App Store has stopped attempting to renew the subscription.
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/is_in_billing_retry_period">(docs)</a></param>
    /// <param name="OfferCodeRefName">The reference name of a subscription offer that you configured in App Store Connect. This field is present when a customer redeemed a subscription offer code. 
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/responsebody/pending_renewal_info">(docs)</a></param>
    /// <param name="OriginalTransactionId">The transaction identifier of the original purchase. <a href="https://developer.apple.com/documentation/appstorereceipts/original_transaction_id">(docs)</a></param>
    /// <param name="PriceConsentStatus">The price consent status for a subscription price increase. This field is only present if the customer was notified of the price increase. 
    /// The default value is <b>false</b> and changes to <b>true</b> if the customer consents.</param>
    /// <param name="ProductId">The unique identifier of the product purchased. You provide this value when creating the product in App Store Connect, 
    /// and it corresponds to the <i>productIdentifier</i> property of the <a href="https://developer.apple.com/documentation/storekit/skpayment">SKPayment</a> object stored in the transaction's payment property.</param>
    public record PendingRenewalInfo(string? AutoRenewProductId, AutoRenewStatus AutoRenewStatus, ExpirationIntent? ExpirationIntent, DateTimeOffset? GracePeriodExpiresDate,
        bool? IsInBillingRetryPeriod, string? OfferCodeRefName, long OriginalTransactionId, bool? PriceConsentStatus, string ProductId);

    /// <summary>
    /// The renewal status for the auto-renewable subscription. <a href="https://developer.apple.com/documentation/appstorereceipts/auto_renew_status">(docs)</a>
    /// </summary>
    public enum AutoRenewStatus
    {
        /// <summary>
        /// The customer has turned off automatic renewal for the subscription.
        /// </summary>
        TurnedOff,

        /// <summary>
        /// The subscription will renew at the end of the current subscription period.
        /// </summary>
        WillRenew,
    }

    /// <summary>
    /// The reason a subscription expired. This field is only present for a receipt that contains an expired auto-renewable subscription.
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/expiration_intent">(docs)</a>
    /// </summary>
    public enum ExpirationIntent
    {
        /// <summary>
        /// The customer voluntarily canceled their subscription.
        /// </summary>
        VoluntarilyCanceled = 1,

        /// <summary>
        /// Billing error; for example, the customer's payment information was no longer valid.
        /// </summary>
        BillingError,

        /// <summary>
        /// The customer did not agree to a recent price increase.
        /// </summary>
        DidNotAgreeToPriceIncrease,

        /// <summary>
        /// The product was not available for purchase at the time of renewal.
        /// </summary>
        ProductWasNotAvailableForPurchase,

        /// <summary>
        /// Unknown error.
        /// </summary>
        UnknownError
    }
}
