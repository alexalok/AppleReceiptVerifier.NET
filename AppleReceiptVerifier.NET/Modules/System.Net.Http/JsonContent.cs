using System.Text;

namespace System.Net.Http
{
    class JsonContent : StringContent
    {
        const string JsonMediaType = "application/json";

        public JsonContent(string jsonContent) : base(jsonContent, Encoding.UTF8, JsonMediaType)
        {
        }
    }
}