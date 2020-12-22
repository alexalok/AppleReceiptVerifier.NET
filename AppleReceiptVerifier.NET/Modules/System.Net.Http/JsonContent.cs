using System.Net.Http;
using System.Text;

namespace AppleReceiptVerifier.NET.Modules.System.Net.Http
{
    class JsonContent : StringContent
    {
        const string JsonMediaType = "application/json";

        public JsonContent(string jsonContent) : base(jsonContent, Encoding.UTF8, JsonMediaType)
        {
        }
    }
}