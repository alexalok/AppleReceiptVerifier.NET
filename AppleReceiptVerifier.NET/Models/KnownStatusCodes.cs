namespace AppleReceiptVerifier.NET.Models
{
    public enum KnownStatusCodes
    {
        Valid = 0,
        MalformedReceiptData = 21002,
        BadAppPassword = 21004,
        ReceiptIsFromTestEnvironment = 21007,
        ReceiptIsFromProductionEnvironment = 21008,
    }
}