using ConsoleApp.Classes;
using ConsoleApp.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleApp.Tests
{
    [TestClass]
    public class MainBootstrapperShould : TestBase
    {
        [TestMethod]
        [Description("Testing sut.Run_WritesToConsole()")]
        [Owner("Emmanuel")]
        [Priority(6)]
        [TestCategory("Platform")]
        [TestCategory("Moq")]

        public void Run_WritesToConsole()
        {
            // Arrange
            var mockConsole = new Mock<IConsoleWrapper>();
            mockConsole.Setup(c => c.WriteLine(It.IsAny<string>()));
            mockConsole.Setup(c => c.Write(It.IsAny<string>()));
            mockConsole.Setup(c => c.ReadLine()).Returns("q");

            var sut = new MainBootstrapper(mockConsole.Object);

            // Act
            sut.Run(System.Array.Empty<string>());

            // Assert
            mockConsole.Verify(c => c.WriteLine("ConsoleApp program started"));
            mockConsole.Verify(c => c.Write("Please enter a choice: "));
        }

        [TestMethod]
        [Description("Testing sut.Run_ClearsConsole()")]
        [Owner("Emmanuel")]
        [Priority(6)]
        [TestCategory("Platform")]
        [TestCategory("Moq")]

        public void Run_ClearsConsole()
        {
            // Arrange
            var mockConsole = new Mock<IConsoleWrapper>();
            mockConsole.SetupSequence(c => c.ReadLine()).Returns("").Returns("q");
            mockConsole.Setup(c => c.Clear());

            var sut = new MainBootstrapper(mockConsole.Object);

            // Act
            sut.Run(System.Array.Empty<string>());

            // Assert
            mockConsole.Verify(c => c.Clear());
        }

    }
}
