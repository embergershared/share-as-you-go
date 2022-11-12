using System.Security.Cryptography.X509Certificates;

namespace ClassLibrary
{
    public class CreditCardApplicationEvaluator
    {
        private readonly IFrequentFlyerValidator _ffValidator;
        private const int AutoReferralMaxAge = 20;
        private const int HighIncomeThreshold = 100_000;
        private const int LowIncomeThreshold = 20_000;

        public CreditCardApplicationEvaluator(IFrequentFlyerValidator ffValidator)
        {
            _ffValidator = ffValidator ?? throw new System.ArgumentNullException(nameof(ffValidator));
        }

        public CreditCardApplicationDecision Evaluate(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            var isValidFF = _ffValidator.IsValid(application.FrequentFlyerNumber);
            if (!isValidFF)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }

    }
}
