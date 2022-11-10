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
        public void FileExists_FileNameDoesExist_ReturnsBool()
        {
            TestContext?.WriteLine("In FileExists_FileNameDoesExist_ReturnsBool() method");

            // Arrange
            var fp = new FileProcess();

            // Act
            TestContext?.WriteLine($"Asserting FileExists for file: {GoodFileName}");
            var actual = GoodFileName != null && fp.FileExists(GoodFileName);

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