using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using ConsoleApp.Classes;
using Microsoft.QualityTools.Testing.Fakes;

namespace ConsoleApp.Tests
{
    [TestClass]
    public class ConsoleWrapperShould : TestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            WriteDescription(GetType());
        }

        [TestMethod]
        [Description("Testing sut.WriteLine()")]
        [Owner("Emmanuel")]
        [Priority(6)]
        [TestCategory("Platform")]
        public void WriteLine_WritesToSystemConsole()
        {
            // Arrange
            const string expected = "Hello to the console";
            var sut = new ConsoleWrapper();

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            sut.WriteLine(expected);
            var actual = sw.ToString().Trim();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Testing sut.Write()")]
        [Owner("Emmanuel")]
        [Priority(6)]
        [TestCategory("Platform")]
        public void Write_WritesToSystemConsole()
        {
            // Arrange
            const string expected = "q";
            var sut = new ConsoleWrapper();

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            sut.Write(expected);
            var actual = sw.ToString().Trim();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Testing sut.ReadKey()")]
        [Owner("Emmanuel")]
        [Priority(6)]
        [TestCategory("Platform")]
        [TestCategory("Shims")]
        public void ReadKey_ReadsFromSystemConsole()
        {
            // Arrange
            using (ShimsContext.Create())
            {
                var expected = new ConsoleKeyInfo(char.Parse("a"), ConsoleKey.A, false, false, false);
                
                // Detour Console.ReadKey()
                System.Fakes.ShimConsole.ReadKey = () => expected;

                var sut = new ConsoleWrapper();

                // Act
                var actual = sut.ReadKey();

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
        
        [TestMethod]
        [Description("Testing sut.ReadLine()")]
        [Owner("Emmanuel")]
        [Priority(6)]
        [TestCategory("Platform")]
        public void ReadLine_ReadsFromSystemConsole()
        {
            // Arrange
            using (ShimsContext.Create())
            {
                const string expected = "Fake Shim data entered";

                // Detour Console.ReadLine()
                System.Fakes.ShimConsole.ReadLine = () => expected;

                var sut = new ConsoleWrapper();

                // Act
                var actual = sut.ReadLine();

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [Description("Testing sut.OutputEncoding property")]
        [Owner("Emmanuel")]
        [Priority(6)]
        [TestCategory("Platform")]
        public void OutputEncoding_GetSetterOperates()
        {
            // Arrange
            var expected = System.Text.Encoding.UTF8;
            var sut = new ConsoleWrapper
            {
                OutputEncoding = expected
            };

            // Act
            var actual = sut.OutputEncoding;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}