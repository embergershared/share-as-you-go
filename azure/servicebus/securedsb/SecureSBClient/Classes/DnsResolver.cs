// <copyright file="DnsResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SecureSBClient.Classes
{
    using DnsClient;
    using Microsoft.Extensions.Logging;
    using Interfaces;

    internal class DnsResolver : IDnsResolver
    {
        private readonly ILogger _logger;

        public DnsResolver(ILogger<DnsResolver> logger)
        {
            _logger = logger;
        }

        public async Task<bool> ResolveAsync(string fqdn)
        {
            _logger.LogInformation("{@method} invoked with: {@parameter}", "ResolveAsync()", fqdn);
            return await DnsResolveAsync(fqdn);
        }

        private async Task<bool> DnsResolveAsync(string fqdn)
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
