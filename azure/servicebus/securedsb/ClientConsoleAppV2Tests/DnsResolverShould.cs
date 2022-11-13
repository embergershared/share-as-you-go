using ClientConsoleAppV2.Classes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClientConsoleAppV2Tests
{
    [TestClass]
    public class DnsResolverShould
    {
        [TestMethod]
        [Description("Test the resolver resolves google.com")]
        public void Resolve_google()
        {
            // Arrange
            var expected = true;
            var logger = Mock.Of<ILogger<DnsResolver>>();
            var mockDnsResolver = new DnsResolver(logger);

            // Act
            var actual = mockDnsResolver.ResolveAsync("www.google.com").Result;

            // Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        [Description("Test the resolver resolves microsoft.com")]
        public void Resolve_microsoft()
        {
            // Arrange
            var expected = true;
            var logger = Mock.Of<ILogger<DnsResolver>>();
            var mockDnsResolver = new DnsResolver(logger);

            // Act
            var actual = mockDnsResolver.ResolveAsync("www.microsoft.com").Result;

            // Assert
            Assert.AreEqual(expected, actual);
        }


    }
}