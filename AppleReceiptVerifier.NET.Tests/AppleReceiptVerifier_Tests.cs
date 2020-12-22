﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AppleReceiptVerifier.NET.Models;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace AppleReceiptVerifier.NET.Tests
{
    public class AppleReceiptVerifier_Tests : BaseTestsClass
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public async Task VerifyReceiptAsync_Fallbacks_To_Test_Environment_Only_When_Explicitly_Set(bool isTestEnvEnabled, bool expectedReceiptValidity)
        {
            var httpHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent(GetVerifiedReceiptJson("Invalid_Receipt_Is_From_Test_Environment"))
                })
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent(GetVerifiedReceiptJson("Valid_Production_Excluding_Old_Subscriptions"))
                });
            var httpClient = new HttpClient(httpHandlerMock.Object);
            var options = new OptionsWrapper<AppleReceiptVerifierOptions>(new AppleReceiptVerifierOptions()
            {
                AppPassword = "test_app_password",
                AcceptTestEnvironmentReceipts = isTestEnvEnabled
            });
            var verifier = new AppleReceiptVerifier(options, httpClient);

            var receipt = await verifier.VerifyReceiptAsync("test_receipt_data", true);

            Assert.Equal(expectedReceiptValidity, receipt.IsValid);
            httpHandlerMock.Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(isTestEnvEnabled ? 2 : 1), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void DeserializeResponse_Valid_Excluding_Old_Subscriptions()
        {
            string json = GetVerifiedReceiptJson("Valid_Production_Excluding_Old_Subscriptions");

            var receipt = AppleReceiptVerifier.DeserializeResponse(json);

            Assert.Equal(KnownStatusCodes.Valid, (KnownStatusCodes) receipt.Status);
            Assert.True(receipt.IsValid);
            Assert.Single(receipt.LatestReceiptInfo!);

            var lastReceiptInfo = receipt.LatestReceiptInfo!.Single();
            Assert.Equal("com.test.app.proYearly", lastReceiptInfo.ProductId);

            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1592834224000), lastReceiptInfo.PurchaseDate);

            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1592575025000), lastReceiptInfo.OriginalPurchaseDate);

            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1624370224000), lastReceiptInfo.ExpiresDate);

            Assert.False(lastReceiptInfo.IsTrialPeriod);
        }

        [Fact]
        public void DeserializeResponse_Invalid_Bad_App_Password()
        {
            string json = GetVerifiedReceiptJson("Invalid_Bad_App_Password");

            var receipt = AppleReceiptVerifier.DeserializeResponse(json);

            Assert.False(receipt.IsValid);
            Assert.Equal(KnownStatusCodes.BadAppPassword, (KnownStatusCodes) receipt.Status);
        }

        [Fact]
        public void DeserializeResponse_Invalid_Malformed_Receipt_Data()
        {
            string json = GetVerifiedReceiptJson("Invalid_Malformed_Receipt_Data");

            var receipt = AppleReceiptVerifier.DeserializeResponse(json);

            Assert.False(receipt.IsValid);
            Assert.Equal(KnownStatusCodes.MalformedReceiptData, (KnownStatusCodes) receipt.Status);
        }
    }
}