namespace ClassLibrary
{
    public class FrequentFlyerValidator : IFrequentFlyerValidator
    {
        // Interface Methods Implementation
        public bool IsValid(string frequentFlyerNumber)
        {
            throw new NotImplementedException("Simulate this dependency.");
        }

        public void IsValid(string frequentFlyerNumber, out bool isValid)
        {
            throw new NotImplementedException("Simulate this dependency.");
        }

        // Interface Properties Implementation
        //public string LicenseKey => throw new NotImplementedException("For demo purposes.");
        public IServiceInformation ServiceInformation => throw new NotImplementedException("For demo purposes.");

        public ValidationMode ValidationMode
        {
            get => throw new NotImplementedException("For demo purposes.");
            set => throw new NotImplementedException("For demo purposes.");
        }

        public event EventHandler? ValidatorLookupPerformed;
    }
}
