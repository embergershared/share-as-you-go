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
        [DataRow("www.google.com", true)]
        [DataRow("www.microsoft.com", true)]
        [DataRow("ww.mikrosoft.com", false)]
        public void Resolve_FQDNs(string fqdn, bool normalResult)
        {
            // Arrange
            var expected = normalResult;
            var logger = Mock.Of<ILogger<DnsResolver>>();
            var sut = new DnsResolver(logger);

            // Act
            var actual = sut.ResolveAsync(fqdn).Result;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}