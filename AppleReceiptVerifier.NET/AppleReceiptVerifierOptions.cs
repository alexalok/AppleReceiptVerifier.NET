namespace AppleReceiptVerifier.NET
{
    public class AppleReceiptVerifierOptions
    {
        public const string ProductionEnvironmentUrl = "https://buy.itunes.apple.com/verifyReceipt";
        public const string TestEnvironmentUrl = "https://sandbox.itunes.apple.com/verifyReceipt";
        public const string DefaultVerifierName = "DefaultVerifier";
        public const string ServicesPrefix = nameof(AppleReceiptVerifier) + "_";

        public string AppPassword { get; set; } = null!;
    }
}