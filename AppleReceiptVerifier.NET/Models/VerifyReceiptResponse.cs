using AppleReceiptVerifierNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleReceiptVerifierNET.Models
{
    /// <summary>
    /// The data returned in the response from the App Store.
    /// </summary>
    /// <param name="Environment">The environment for which the receipt was generated.</param>
    /// <param name="IsRetryable">An indicator that an error occurred during the request. A value of <b>true</b> indicates a temporary issue; 
    /// retry validation for this receipt at a later time. A value of <b>false</b> indicates an unresolvable issue; 
    /// do not retry validation for this receipt. Only applicable to status codes 21100-21199.</param>
    /// <param name="LatestReceipt">The latest Base64 encoded app receipt. Only returned for receipts that contain auto-renewable subscriptions.</param>
    /// <param name="LatestReceiptInfo">An array that contains all in-app purchase transactions. This excludes transactions for consumable products that have been marked as finished by your app. 
    /// Only returned for receipts that contain auto-renewable subscriptions. 
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/responsebody/latest_receipt_info">(docs)</a></param>
    /// <param name="PendingRenewalInfo">An array of elements that refers to auto-renewable subscription renewals that are open or failed in the past. 
    /// <a href="https://developer.apple.com/documentation/appstorereceipts/responsebody/pending_renewal_info">(docs)</a></param>
    /// <param name="Receipt">A JSON representation of the receipt that was sent for verification.</param>
    /// <param name="Status">Either 0 if the receipt is valid, or a status code if there is an error. The status code reflects the status of the app receipt as a whole. 
    /// See <a href="https://developer.apple.com/documentation/appstorereceipts/status">status</a> for possible status codes and descriptions.</param>
    public record VerifyReceiptResponse(Environment? Environment, bool? IsRetryable, string? LatestReceipt, ReceiptInfo[]? LatestReceiptInfo, PendingRenewalInfo[]? PendingRenewalInfo, Receipt Receipt, int Status)
    {
        /// <summary>
        /// <b>true</b> if the receipt is valid, <b>false</b> if there is an error.
        /// </summary>
        public bool IsValid => (KnownStatusCodes) Status == KnownStatusCodes.Valid;

        /// <summary>
        /// Get an error description. Use if <see cref="Status"/> != 0.
        /// Click <a href="https://developer.apple.com/documentation/appstorereceipts/status">here</a> for reference.
        /// </summary>
        public string ErrorDescription => Status switch
        {
            0 => "No error.",
            21000 => "The request to the App Store was not made using the HTTP POST request method.",
            21001 => "This status code is no longer sent by the App Store.",
            21002 => "The data in the receipt-data property was malformed or the service experienced a temporary issue. Try again.",
            21003 => "The receipt could not be authenticated.",
            21004 => "The shared secret you provided does not match the shared secret on file for your account.",
            21005 => "The receipt server was temporarily unable to provide the receipt. Try again.",
            21006 => "This receipt is valid but the subscription has expired. When this status code is returned to your server, the receipt data is also decoded and returned as part of the response. Only returned for iOS 6-style transaction receipts for auto-renewable subscriptions.",
            21007 => "This receipt is from the test environment, but it was sent to the production environment for verification.",
            21008 => "This receipt is from the production environment, but it was sent to the test environment for verification.",
            21009 => "Internal data access error. Try again later.",
            21010 => "The user account cannot be found or has been deleted.",
            >= 21100 and <= 21199 => "Internal data access error.",
            _ => "Unknown error."
        };
    }
}
