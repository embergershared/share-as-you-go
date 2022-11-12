using Xunit;
using Moq;

namespace ClassLibrary.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AutoAccept_HighIncomeApplications()
        {
            // Arrange
            Mock<IFrequentFlyerValidator> mockFfValidator = new Mock<IFrequentFlyerValidator>();
            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
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
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();
            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
            var application = new CreditCardApplication { Age = 19 };
            var expected = CreditCardApplicationDecision.ReferredToHuman;

            // Act
            var actual = sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AutoDecline_LowIncomeApplications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();
            // mockFfValidator.Setup(m => m.IsValid("x")).Returns(true);
            //mockFfValidator.Setup(
            //        m => m.IsValid(It.IsAny<string>()))
            //        .Returns(true);
            //mockFfValidator.Setup(
            //        m => m.IsValid(
            //            It.Is<string>(text => text.StartsWith("t"))))
            //    .Returns(true);
            //mockFfValidator.Setup(
            //        m => m.IsValid(
            //            It.IsInRange<string>("a", "h", Range.Inclusive)))
            //    .Returns(true);
            //mockFfValidator.Setup(
            //        m => m.IsValid(
            //            It.IsIn<string>("a", "x", "test")))
            //    .Returns(true);
            mockFfValidator.Setup(
                    m => m.IsValid(
                        It.IsRegex("[a-z]")))
                .Returns(true);


            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999, Age = 42, FrequentFlyerNumber = "y"
            };
            var expected = CreditCardApplicationDecision.AutoDeclined;

            // Act
            var actual = sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, actual);
        }

    }
}