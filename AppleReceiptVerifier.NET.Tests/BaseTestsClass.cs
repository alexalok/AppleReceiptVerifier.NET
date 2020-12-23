using System.IO;
using System.Runtime.CompilerServices;

namespace AppleReceiptVerifierNET.Tests
{
    public abstract class BaseTestsClass
    {
        const string ResponsesFolderName = "Responses";

        protected static string GetVerifiedReceiptJson([CallerMemberName] string fileNameNoExtension = null!)
        {
            string path = Path.Combine(ResponsesFolderName, fileNameNoExtension + ".json");
            return File.ReadAllText(path);
        }
    }
}