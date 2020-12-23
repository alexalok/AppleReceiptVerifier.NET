namespace AppleReceiptVerifier.NET
{
    public class AppleReceiptVerifierOptions
    {
        public const string ProductionEnvironmentUrl = "https://buy.itunes.apple.com/verifyReceipt";
        public const string TestEnvironmentUrl = "https://sandbox.itunes.apple.com/verifyReceipt";
        internal const string DefaultVerifierName = "DefaultVerifier";
        internal const string ServicesPrefix = nameof(AppleReceiptVerifier) + "_";

        public string AppPassword { get; set; } = null!;

        /// <summary>
        /// Indicates whether the verifier should check the receipts with the test environment server.
        /// <b>Caution!</b> Setting this to <i>true</i> will cause test receipts to be marked as valid.
        /// Should be set to <i>true</i> only in the development environments.
        /// </summary>
        public bool AcceptTestEnvironmentReceipts { get; set; }
    }
}