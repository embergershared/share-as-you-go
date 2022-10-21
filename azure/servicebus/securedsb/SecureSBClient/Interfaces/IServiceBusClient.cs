// <copyright file="IServiceBusClient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Globalization;

namespace SecureSBClient.Interfaces
{
    internal interface IServiceBusClient
    {
        bool CreateClient(string sbNamespace, string? clientId = null);
        Task DisposeClientAsync();

        Task<bool> SendMessageAsync(string queue, string message);
    }
}
