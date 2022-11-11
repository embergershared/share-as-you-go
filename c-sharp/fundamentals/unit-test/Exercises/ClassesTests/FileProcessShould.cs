using System;
using ClassesLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassesTests
{
    [TestClass]
    public class FileProcessShould : TestBase
    {
        private const string BadFileName = @"C:\Windows\bogus.exc";

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            // TODO: Initialize stuff for all tests in this class
            context.WriteLine($"In ClassInitialize() method of Class: FileProcessShould");
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            // TODO: Cleanup after this class tests
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestContext?.WriteLine("In TestInitialize() method");

            if (TestContext != null && TestContext.TestName.StartsWith("FileExists_FileNameDoesExist"))
            {
                SetGoodFileName();
                EnsureGoodFileExists();
            }

            WriteDescription(GetType());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestContext?.WriteLine("In TestCleanup() method");

            if (TestContext != null && TestContext.TestName.StartsWith("FileExists_FileNameDoesExist"))
            {
                DeleteGoodFile();
            }
        }


        #region Tests methods
        [TestMethod]
        [Description("Check if the file exists.")]
        [Owner("Manu")]
        [Priority(1)]
        [TestCategory("NormalValues")]
        // [Ignore]
        public void FileExists_FileNameDoesExist_ReturnsBool()
        {
            TestContext?.WriteLine("In FileExists_FileNameDoesExist_ReturnsBool() method");

            // Arrange
            var fp = new FileProcess();

            // Act
            TestContext?.WriteLine($"Asserting FileExists for file: {GoodFileName}");
            var actual = GoodFileName != null && fp.FileExists(GoodFileName);

            // Assert
            Assert.IsTrue(actual, "File \"{0}\" does not exist.", GoodFileName);
        }

        [TestMethod]
        [Description("Check if the file does not exist.")]
        [Owner("Manu")]
        [Priority(1)]
        [TestCategory("NormalValues")]
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
        [Description("Check for a thrown ArgumentNullException using ExpectedException.")]
        [Owner("Jack")]
        [Priority(2)]
        [TestCategory("Exception")]
        public void FileExists_FileNameIsNullOrEmptyUsingAttribute_ThrowException()
        {
            // Arrange
            var fp = new FileProcess();

            // Act
            fp.FileExists("");

            // Assert
        }

        [TestMethod]
        [Description("Check for a thrown ArgumentNullException using try-catch.")]
        [Owner("Manu")]
        [Priority(1)]
        [TestCategory("Exception")]
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

        [TestMethod]
        [Description("Test the Timeout() attribute")]
        [Timeout(1000)]
        public void SimulateTimeout()
        {
            System.Threading.Thread.Sleep(500);
        }

        [TestMethod]
        [Description("Test the DataRow() attribute")]
        [DataRow(1, 1, DisplayName = "First equal test with (1,1)")]
        [DataRow(62, 62, DisplayName = "Second equal test with (62,62)")]
        public void Assert_AreNumberEquals(int num1, int num2)
        {
            Assert.AreEqual(num1, num2);
        }


        [TestMethod]
        [Description("Test the DeploymentItem() attribute")]
        [DeploymentItem("FileToDeploy.txt")]
        [DataRow(@"C:\Windows\explorer.exe", DisplayName = "explorer.exe")]
        [DataRow("FileToDeploy.txt", DisplayName = "DeploymentItem: FileToDeploy.txt")]
        public void FileExists_FileNameUsingDeploymentItem_ReturnsBool(string fileName)
        {
            TestContext?.WriteLine("In FileExists_FileNameUsingDeploymentItem_ReturnsBool() method");

            // Arrange
            var fp = new FileProcess();
                // If fileName doesn't have "\", we assume it is the file we carry with DeploymentItem so we add its Path prefix
            if (!fileName.Contains($@"\"))
            {
                if (TestContext != null) fileName = TestContext.DeploymentDirectory + @"\" + fileName;
            }

            // Act
            TestContext?.WriteLine($"Asserting FileExists for file: {fileName}");
            var actual = fp.FileExists(fileName);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [Description("Try different String equality tests.")]
        public void Strings_AreEqual()
        {
            // Arrange
            const string str1 = "Jack";
            const string str2 = "jack";

            // Act

            // Assert
            Assert.AreEqual(str1,str2, true);
        }


        [TestMethod]
        [Description("Try different Objects are same tests.")]
        public void Objects_AreSame()
        {
            // Arrange
            var x = new FileProcess();
            var y = x;
            var z = new FileProcess();

            // Act

            // Assert
            Assert.AreSame(x, y);
            Assert.AreNotSame(y, z);
        }

        #endregion
    }
}