using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AppleReceiptVerifierNET.Models;
using AppleReceiptVerifierNET.Modules.System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppleReceiptVerifierNET
{
    public interface IAppleReceiptVerifier
    {
        /// <summary>
        /// Send a receipt to the App Store for verification.
        /// </summary>
        /// <param name="receiptData">The Base64-encoded receipt data.</param>
        /// <param name="excludeOldTransactions">Set this value to <b>true</b> for the response to include only the latest renewal transaction for any subscriptions. Default: <b>false</b>.</param>
        Task<VerifyReceiptResponse> VerifyReceiptAsync(string receiptData, bool excludeOldTransactions = false);
    }

    public class AppleReceiptVerifier : IAppleReceiptVerifier
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new QuotedConverter(),
                new CustomEnumConverterFactory(),
                new TimestampToDateTimeOffsetConverter()
            }
        };

        internal readonly AppleReceiptVerifierOptions Options;
        readonly HttpClient _httpClient;

        [ActivatorUtilitiesConstructor]
        public AppleReceiptVerifier(IOptions<AppleReceiptVerifierOptions> options, HttpClient httpClient)
        {
            Options = options.Value;
            _httpClient = httpClient;
        }

        public AppleReceiptVerifier(AppleReceiptVerifierOptions options, HttpClient httpClient)
        {
            Options = options;
            _httpClient = httpClient;
        }

        public async Task<VerifyReceiptResponse> VerifyReceiptAsync(string receiptData, bool excludeOldTransactions = false)
        {
            var requestObj = new VerifyReceiptRequest(receiptData, Options.AppSecret, excludeOldTransactions);
            string requestJson = JsonSerializer.Serialize(requestObj);
            var verifiedReceipt = await VerifyReceiptInternalAsync(AppleReceiptVerifierOptions.ProductionEnvironmentUrl, requestJson).ConfigureAwait(false);
            if ((KnownStatusCodes)verifiedReceipt.Status == KnownStatusCodes.ReceiptIsFromTestEnvironment && Options.AcceptTestEnvironmentReceipts)
                verifiedReceipt = await VerifyReceiptInternalAsync(AppleReceiptVerifierOptions.TestEnvironmentUrl, requestJson).ConfigureAwait(false);
            return verifiedReceipt;
        }

        async Task<VerifyReceiptResponse> VerifyReceiptInternalAsync(string environmentUrl, string requestBodyJson)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, environmentUrl);
            req.Content = new JsonContent(requestBodyJson);
            string rawResp = await (await _httpClient.SendAsync(req).ConfigureAwait(false))
                .Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var verifiedReceipt = DeserializeResponse(rawResp);
            return verifiedReceipt;
        }

        internal static VerifyReceiptResponse DeserializeResponse(string rawJson)
        {
            var resp = JsonSerializer.Deserialize<VerifyReceiptResponse>(rawJson, JsonSerializerOptions)!;
            resp.RawJson = rawJson;
            return resp;
        }
    }
}