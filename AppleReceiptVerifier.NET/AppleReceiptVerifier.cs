using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppleReceiptVerifier.NET
{
    public interface IAppleReceiptVerifier
    {
        Task<VerifiedReceipt> VerifyReceiptAsync(string receiptData, bool excludeOldTransactions = false);
    }

    public class AppleReceiptVerifier : IAppleReceiptVerifier
    {
        internal readonly AppleReceiptVerifierOptions Options;
        readonly HttpClient _httpClient;

        public AppleReceiptVerifier(IOptions<AppleReceiptVerifierOptions> options, HttpClient httpClient)
        {
            Options = options.Value;
            _httpClient = httpClient;
        }

        public async Task<VerifiedReceipt> VerifyReceiptAsync(string receiptData, bool excludeOldTransactions = false)
        {
            var requestObj = new VerifyReceiptRequest(receiptData, Options.AppPassword, excludeOldTransactions);
            string requestJson = JsonSerializer.Serialize(requestObj);
            var verifiedReceipt = await VerifyReceiptInternalAsync(AppleReceiptVerifierOptions.ProductionEnvironmentUrl, requestJson).ConfigureAwait(false);
            if ((KnownStatusCodes) verifiedReceipt.Status == KnownStatusCodes.ReceiptIsFromTestEnvironment)
                verifiedReceipt = await VerifyReceiptInternalAsync(AppleReceiptVerifierOptions.TestEnvironmentUrl, requestJson).ConfigureAwait(false);
            return verifiedReceipt;
        }

        async Task<VerifiedReceipt> VerifyReceiptInternalAsync(string environmentUrl, string requestBodyJson)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, environmentUrl);
            req.Content = new JsonContent(requestBodyJson);
            string? rawResp = await (await _httpClient.SendAsync(req).ConfigureAwait(false))
                .Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var verifiedReceipt = VerifiedReceipt.DeserializeJson(rawResp);
            return verifiedReceipt;
        }
    }
}