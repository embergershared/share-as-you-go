using ClassesLibrary;

namespace ClassesTests
{
    [TestClass]
    public class FileProcessShould
    {
        [TestMethod]
        public void FileExists_FileNameExists_ReturnsBool()
        {
            // Arrange
            var fp = new FileProcess();

            // Act
            var actual = fp.FileExists(@"C:\Windows\explorer.exe");

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void FileExists_FileNameDoesNotExist_ReturnsBool()
        {
            // Arrange
            var fp = new FileProcess();

            // Act
            var actual = fp.FileExists(@"C:\Windows\bogus.exc");

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
            var actual = fp.FileExists("");

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
                var actual = fp.FileExists("");
            }
            catch (ArgumentNullException)
            {
                // Test succeeded
                return;
            }

            // Test failed
            Assert.Fail("Call to FileExists did NOT throw an ArgumentNullException");
        }
    }
}