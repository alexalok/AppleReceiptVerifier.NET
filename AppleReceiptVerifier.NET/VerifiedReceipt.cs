using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppleReceiptVerifier.NET
{
    public class VerifiedReceipt
    {
        static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new QuotedLongConverter(), new QuotedBoolConverter() }
        };

        public int Status { get; set; }

        public bool IsValid => (KnownStatusCodes) Status == KnownStatusCodes.Valid;

        [JsonPropertyName("latest_receipt_info")]
        public List<ReceiptInfo>? LatestReceiptInfos { get; set; }

        public ReceiptInfo? LastReceiptInfo => LatestReceiptInfos?.LastOrDefault();

        /// <summary>
        /// Get an error description. Use if <see cref="Status"/> != 0.
        /// Click <a href="https://developer.apple.com/documentation/appstorereceipts/status">here</a> for reference.
        /// </summary>
        public string GetErrorDescription() => Status switch
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
            int x when x >= 21100 && x <= 21199 => "Internal data access error.",
            _ => "Unknown error."
        };

        public static VerifiedReceipt DeserializeJson(string jsonString)
        {
            var receipt = JsonSerializer.Deserialize<VerifiedReceipt>(jsonString, JsonSerializerOptions);
            Debug.Assert(receipt != null);
            return receipt;
        }
    }

    public class ReceiptInfo
    {
        [JsonPropertyName("product_id")]
        public string ProductId { get; set; } = null!;

        [JsonPropertyName("purchase_date_ms")]
        public long PurchaseDateMs { get; set; }

        public DateTimeOffset PurchaseDateUtc => DateTimeOffset.FromUnixTimeMilliseconds(PurchaseDateMs);

        [JsonPropertyName("original_purchase_date_ms")]
        public long OriginalPurchaseDateMs { get; set; }

        public DateTimeOffset OriginalPurchaseDateUtc => DateTimeOffset.FromUnixTimeMilliseconds(OriginalPurchaseDateMs);

        [JsonPropertyName("expires_date_ms")]
        public long ExpiresDateMs { get; set; }

        public DateTimeOffset ExpiresDateUtc => DateTimeOffset.FromUnixTimeMilliseconds(ExpiresDateMs);

        [JsonPropertyName("is_trial_period")]
        public bool IsTrialPeriod { get; set; }
    }

    public enum KnownStatusCodes
    {
        Valid = 0,
        MalformedReceiptData = 21002,
        BadAppPassword = 21004,
        ReceiptIsFromTestEnvironment = 21007,
        ReceiptIsFromProductionEnvironment = 21008,
    }
}