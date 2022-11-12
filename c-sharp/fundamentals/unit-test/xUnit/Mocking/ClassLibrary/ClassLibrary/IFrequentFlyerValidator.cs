namespace ClassLibrary
{
    public interface IFrequentFlyerValidator
    {
        // Interface Methods
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
        
        // Interface Properties
        string LicenseKey { get; }
    }
}
