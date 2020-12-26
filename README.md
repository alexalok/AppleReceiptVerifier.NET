# 📝 What is AppleReceiptVerifier.NET?

AppleReceiptVerifier.NET is a library used to verify the App Store receipts with the App Store verification service. 

You can read more about receipt verification on the [official documentation website](https://developer.apple.com/documentation/appstorereceipts).

## ✔️ Which .NET versions and OS platforms are supported?

The library targets .NET Standard 2.0 and .NET 5 which makes it compatible with the following .NET implementations:

* .NET Framework 4.6.1 and above;
* .NET Core 2.0 and above;
* .NET 5;
* and others, see [.NET implementation support](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support).

The library is platform-agnostic and can run on **any OS** supported by the selected .NET implementation.

# ℹ️ Usage
To use the library you need an iOS or MacOS application that has in-app purchases. You also need to get the app shared secret from the App Store Connect [(how?)](https://developers.facebook.com/docs/app-events/getting-started-app-events-ios/app-shared-secret/). 
Next, you need to decide whether you want to use the library's DI integration. If not, refer to the **Standalone** section below. Otherwise, scroll down to the **DI Integration** section.

## 📦 Standalone

The verifier can be used without the DI container. Simply instantiate it like this:

```c#
string receipt = "[[very long base64encoded string]]";
var verifierOptions = new AppleReceiptVerifierOptions()
{
    AppSecret = "your app shared secret",
    AcceptTestEnvironmentReceipts = false // don't trust test environment receipts in production!
};
var httpClient = new HttpClient();
var verifier = new AppleReceiptVerifier(verifierOptions, httpClient);
var response = await verifier.VerifyReceiptAsync(receipt, true);
if (!response.IsValid)
{
    Console.WriteLine("Failed to verify the receipt: " + response.ErrorDescription);
}
else
{
    Console.WriteLine("Receipt is valid");
}
```

## ⚙️ DI Integration

The library supports integration with the `Microsoft.Extensions.DependencyInjection` DI implementation. 

### If you want to use a single verifier to verify receipts from a single app, take the following steps:

1. Register a verifier in your `Startup.cs`. 

   ```c#
   services.AddAppleReceiptVerifier(c =>
   {
       c.AppSecret = "your app shared secret";
       c.AcceptTestEnvironmentReceipts = false; // don't trust test environment receipts in production!
   });
   ```

2. Inject `IAppleReceiptVerifier` in your controller or a service:

   ```c#
    public class ExampleController
    {
        readonly IAppleReceiptVerifier _verifier;

        public ExampleController(IAppleReceiptVerifier verifier)
        {
            _verifier = verifier;
        }

        public async Task<IActionResult> CheckReceiptAsync(string receipt)
        {
            var response = await _verifier.VerifyReceiptAsync(receipt, true);
            if (!response.IsValid)
            {
                return BadRequest(response.ErrorDescription);
            }
            else
            {
                return Ok();
            }
        }
    }
   ```

### If you want to support multiple verifiers, for multiple applications, things are a little bit more tricky:

1. Register multiple verifiers in your `Startup.cs`, each with a unique name:

   ```c#
    services.AddAppleReceiptVerifier(c =>
    {
      c.AppSecret = "first app shared secret";
      c.AcceptTestEnvironmentReceipts = true;
    }, "App1");

    services.AddAppleReceiptVerifier(c =>
    {
      c.AppSecret = "second app shared secret";
      c.AcceptTestEnvironmentReceipts = true;
    }, "App2");
   ```

2. Inject `IAppleReceiptVerifierResolver` in your constructor and resolve the registered verifiers by their names:

   ```c#
    public class ExampleController
    {
        readonly IAppleReceiptVerifier _verifier1;
        readonly IAppleReceiptVerifier _verifier2;

        public ExampleController(IAppleReceiptVerifierResolver verifierResolver)
        {
            _verifier1 = verifierResolver.Resolve("App1");
            _verifier2 = verifierResolver.Resolve("App2");
        }

        public async Task<IActionResult> CheckReceiptAsync(string receipt, string appName)
        {
            var response = await (appName switch
            {
                "App1" => _verifier1.VerifyReceiptAsync(receipt, true),
                "App2" => _verifier2.VerifyReceiptAsync(receipt, true),
                _ => throw new ArgumentOutOfRangeException(nameof(appName), appName, "Invalid app name.")
            });
            if (!response.IsValid)
            {
                return BadRequest(response.ErrorDescription);
            }
            else
            {
                return Ok();
            }
        }
    }
   ```

# 🤔 Advanced usage
Please refer to the [official documentation](https://developer.apple.com/documentation/appstorereceipts/verifyreceipt) to learn more about how different receipt fields can be used.
