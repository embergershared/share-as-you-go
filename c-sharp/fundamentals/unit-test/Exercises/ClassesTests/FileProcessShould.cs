using ClassesLibrary;

namespace ClassesTests
{
    [TestClass]
    public class FileProcessShould : TestBase
    {
        private const string BadFileName = @"C:\Windows\bogus.exc";

        #region Tests methods
        [TestMethod]
        public void FileExists_FileNameDoesExist_ReturnsBool()
        {
            // Arrange
            var fp = new FileProcess();
            SetGoodFileName();
            EnsureGoodFileExists();

            // Act
            TestContext?.WriteLine($"Checking file: {GoodFileName}");
            var actual = GoodFileName != null && fp.FileExists(GoodFileName);
            DeleteGoodFile();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void FileExists_FileNameDoesNotExist_ReturnsBool()
        {
            // Arrange
            var fp = new FileProcess();

            // Act
            var actual = fp.FileExists(BadFileName);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileExists_FileNameIsNullOrEmptyUsingAttribute_ThrowException()
        {
            // Arrange
            var fp = new FileProcess();

            // Act
            fp.FileExists("");

            // Assert
        }

        [TestMethod]
        public void FileExists_FileNameIsNullOrEmptyUsingTryCatch_ThrowException()
        {
            // Arrange
            var fp = new FileProcess();

            // Act
            try
            {
                fp.FileExists("");
            }
            catch (ArgumentNullException)
            {
                // Test succeeded
                return;
            }

            // Test failed
            Assert.Fail("Call to FileExists did NOT throw an ArgumentNullException");
        }
        #endregion
    }
}