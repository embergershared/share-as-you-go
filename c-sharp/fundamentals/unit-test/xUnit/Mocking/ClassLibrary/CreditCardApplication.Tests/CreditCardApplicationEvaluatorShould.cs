using Xunit;

namespace ClassLibrary.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AutoAccept_HighIncomeApplications()
        {
            // Arrange
            var sut = new CreditCardApplicationEvaluator();
            var application = new CreditCardApplication { GrossAnnualIncome = 110_000 };
            var expected = CreditCardApplicationDecision.AutoAccepted;

            // Act
            var actual = sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Refer_YoungApplications()
        {
            // Arrange
            var sut = new CreditCardApplicationEvaluator();
            var application = new CreditCardApplication { Age = 19 };
            var expected = CreditCardApplicationDecision.ReferredToHuman;

            // Act
            var actual = sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, actual);
        }

    }
}