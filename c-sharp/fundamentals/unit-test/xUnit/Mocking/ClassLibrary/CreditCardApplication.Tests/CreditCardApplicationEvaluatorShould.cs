using Xunit;
using Moq;

namespace ClassLibrary.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AutoAccept_HighIncome_Applications()
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
        public void Refer_Young_Applications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();
            mockFfValidator.DefaultValue = DefaultValue.Mock;

            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
            var application = new CreditCardApplication { Age = 19 };
            var expected = CreditCardApplicationDecision.ReferredToHuman;

            // Act
            var actual = sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AutoDecline_LowIncome_Applications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();

            //mockFfValidator.Setup(m => m.IsValid("x")).Returns(true);
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
            mockFfValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");


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

        [Fact]
        public void Refer_InvalidFrequentFlyer_Applications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>(MockBehavior.Loose);
            mockFfValidator.Setup(
                x => x.IsValid(It.IsAny<string>()))
                .Returns(false);
            mockFfValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
            var application = new CreditCardApplication();
            var expected = CreditCardApplicationDecision.ReferredToHuman;

            // Act
            var actual = sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, actual);
        }

        //[Fact]
        //public void AutoDecline_LowIncome_Applications_UsingOutEvaluation()
        //{
        //    // Arrange
        //    var expected = CreditCardApplicationDecision.AutoDeclined;

        //    var mockFfValidator = new Mock<IFrequentFlyerValidator>();
        //    var isValid = true;
        //    mockFfValidator.Setup(
        //        x => x.IsValid(It.IsAny<string>(), out isValid));

        //    var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
        //    var application = new CreditCardApplication
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42,
        //        FrequentFlyerNumber = "y"
        //    };

        //    // Act
        //    var actual = sut.EvaluateUsingOut(application);

        //    // Assert
        //    Assert.Equal(expected, actual);
        //}

        [Fact]
        public void Refer_WhenLicensedKeyExpired()
        {
            // Arrange
            var expected = CreditCardApplicationDecision.ReferredToHuman;
            var mockValidator = new Mock<IFrequentFlyerValidator>();
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(x => x.LicenseKey).Returns(GetLicenseKey);

            //// Heavy mock creation for LicenseKey
            //var mockLicenseData = new Mock<ILicenseData>();
            //mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED");
            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

            // Light mock creation for LicenseKey
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 42 };

            // Act
            var actual = sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, actual);
        }

        //private string GetLicenseKey()
        //{
        //    return "EXPIRED";
        //}

        [Fact]
        public void UseDetailedValidationMode_ForOlderApplications()
        {
            // Arrange
            var expected = ValidationMode.Detailed;
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();
            mockFfValidator.SetupAllProperties();

            mockFfValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockFfValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //mockFfValidator.SetupProperty(x => x.ValidationMode);

            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
            var application = new CreditCardApplication { Age = 42 };

            // Act
            sut.Evaluate(application);

            // Assert
            Assert.Equal(expected, mockFfValidator.Object.ValidationMode);
        }

        [Fact]
        public void ValidateFrequentFlyerNum_ForLowIncome_Applications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();

            mockFfValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
            var application = new CreditCardApplication();

            // Act
            sut.Evaluate(application);

            // Assert
            mockFfValidator.Verify(x => x.IsValid(It.IsAny<string>()),
                 Times.Once, "Frequent flyer number should be verified once");
        }

        [Fact]
        public void NotValidateFrequentFlyerNum_ForHighIncome_Applications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();
            mockFfValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            
            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);
            
            var application = new CreditCardApplication{ GrossAnnualIncome = 120_000 };

            // Act
            sut.Evaluate(application);

            // Assert
            mockFfValidator.Verify(x => x.IsValid(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void CheckLicenseKey_ForLowIncome_Applications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();
            mockFfValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 20_000 };

            // Act
            sut.Evaluate(application);

            // Assert
            mockFfValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        [Fact]
        public void SetDetailedValidationMode_ForOlder_Applications()
        {
            // Arrange
            var mockFfValidator = new Mock<IFrequentFlyerValidator>();
            mockFfValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockFfValidator.Object);

            var application = new CreditCardApplication { Age = 35 };

            // Act
            sut.Evaluate(application);

            // Assert
            mockFfValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>());
            //mockFfValidator.VerifyNoOtherCalls();
        }


    }
}