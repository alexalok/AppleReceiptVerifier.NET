using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace AppleReceiptVerifier.NET.Tests
{
    public class VerifiedReceipt_Tests : BaseTestsClass
    {

        [Fact]
        public void Valid_Excluding_Old_Subscriptions()
        {
            string json = GetVerifiedReceiptJson();

            var receipt = VerifiedReceipt.DeserializeJson(json);

            Assert.Equal(KnownStatusCodes.Valid, (KnownStatusCodes) receipt.Status);
            Assert.True(receipt.IsValid);
            Assert.Single(receipt.LatestReceiptInfos!);

            var lastReceiptInfo = receipt.LastReceiptInfo!;
            Assert.Equal("com.test.app.proYearly", lastReceiptInfo.ProductId);

            Assert.Equal(1592834224000, lastReceiptInfo.PurchaseDateMs);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1592834224000), lastReceiptInfo.PurchaseDateUtc);

            Assert.Equal(1592575025000, lastReceiptInfo.OriginalPurchaseDateMs);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1592575025000), lastReceiptInfo.OriginalPurchaseDateUtc);

            Assert.Equal(1624370224000, lastReceiptInfo.ExpiresDateMs);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1624370224000), lastReceiptInfo.ExpiresDateUtc);

            Assert.False(lastReceiptInfo.IsTrialPeriod);
        }

        [Fact]
        public void Invalid_Bad_App_Password()
        {
            string json = GetVerifiedReceiptJson();

            var receipt = VerifiedReceipt.DeserializeJson(json);

            Assert.False(receipt.IsValid);
            Assert.Equal(KnownStatusCodes.BadAppPassword, (KnownStatusCodes) receipt.Status);
        }

        [Fact]
        public void Invalid_Malformed_Receipt_Data()
        {
            string json = GetVerifiedReceiptJson();

            var receipt = VerifiedReceipt.DeserializeJson(json);

            Assert.False(receipt.IsValid);
            Assert.Equal(KnownStatusCodes.MalformedReceiptData, (KnownStatusCodes) receipt.Status);
        }

        
    }
}