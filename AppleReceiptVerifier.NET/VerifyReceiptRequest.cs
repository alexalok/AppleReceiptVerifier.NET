using System;
using System.Text.Json.Serialization;

namespace AppleReceiptVerifier.NET
{
    class VerifyReceiptRequest
    {
        [JsonPropertyName("receipt-data")]
        public string ReceiptData { get; }

        [JsonPropertyName("password")]
        public string Password { get; }

        [JsonPropertyName("exclude-old-transactions")]
        public bool ExcludeOldTransactions { get; }

        public VerifyReceiptRequest(string receiptData, string password, bool excludeOldTransactions)
        {
            ReceiptData = receiptData ?? throw new ArgumentNullException(nameof(receiptData));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            ExcludeOldTransactions = excludeOldTransactions;
        }
    }
}