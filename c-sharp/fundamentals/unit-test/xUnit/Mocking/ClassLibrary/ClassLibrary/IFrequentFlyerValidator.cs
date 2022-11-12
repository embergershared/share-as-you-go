namespace ClassLibrary
{
    public interface IFrequentFlyerValidator
    {
        // Interface Methods
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);

        // Interface Properties
        // string LicenseKey { get; }
        IServiceInformation ServiceInformation { get; }

        ValidationMode ValidationMode { get; set; }
    }

    public interface ILicenseData
    {
        string LicenseKey { get; }
    }

    public interface IServiceInformation
    {
        ILicenseData License { get; }
    }
}
