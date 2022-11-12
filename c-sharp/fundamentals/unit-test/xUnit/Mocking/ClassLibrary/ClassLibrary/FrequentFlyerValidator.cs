namespace ClassLibrary
{
    public class FrequentFlyerValidator : IFrequentFlyerValidator
    {
        public bool IsValid(string frequentFlyerNumber)
        {
            throw new NotImplementedException("Simulate this dependency.");
        }

        public void IsValid(string frequentFlyerNumber, out bool isValid)
        {
            throw new NotImplementedException("Simulate this dependency.");
        }

        public string LicenseKey => throw new NotImplementedException("For demo purposes.");
    }
}
