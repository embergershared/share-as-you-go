// <copyright file="DnsResolver.cs" company="PlaceholderCompany">
//
// DISCLAIMER
//
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// </copyright>

using DnsClient;
using Microsoft.Extensions.Logging;
using SecureSBClient.Interfaces;

namespace SecureSBClient.Classes
{
    internal class DnsResolver : IDnsResolver
    {
        // Private members
        private readonly ILogger _logger;

        // Constructor
        public DnsResolver(ILogger<DnsResolver> logger)
        {
            _logger = logger;
        }

        // Interface implementation
        public async Task<bool> ResolveAsync(string fqdn)
        {
            _logger.LogInformation("Resolving: {@fqdn}", fqdn);
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(fqdn, QueryType.A);

            if (result.HasError)
            {
                _logger.LogError("Impossible to resolve {@fqdn}", fqdn);
                return false;
            }
            else
            {
                _logger.LogInformation("Results from DNS Server: {@ns_name}", result.NameServer.ToString());
                foreach (var nsRecord in result.Answers)
                {
                    _logger.LogInformation("Record: {@ns_record}", nsRecord.ToString());
                }

                return true;
            }
        }
    }
}
