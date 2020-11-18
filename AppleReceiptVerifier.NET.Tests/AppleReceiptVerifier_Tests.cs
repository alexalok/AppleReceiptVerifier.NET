using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
                    Content = new StringContent(GetVerifiedReceiptJson("Valid_Excluding_Old_Subscriptions"))
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
    }
}