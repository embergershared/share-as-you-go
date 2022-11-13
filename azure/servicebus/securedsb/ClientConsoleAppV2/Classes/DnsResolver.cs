// <copyright file="DnsResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using ClientConsoleAppV2.Interfaces;
using DnsClient;
using Microsoft.Extensions.Logging;

namespace ClientConsoleAppV2.Classes
{
    public class DnsResolver : IDnsResolver
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
