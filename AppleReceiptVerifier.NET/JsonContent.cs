using System.Net.Http;
using System.Text;

namespace AppleReceiptVerifier.NET
{
    class JsonContent : StringContent
    {
        const string JsonMediaType = "application/json";

        public JsonContent(string jsonContent) : base(jsonContent, Encoding.UTF8, JsonMediaType)
        {
        }
    }
}