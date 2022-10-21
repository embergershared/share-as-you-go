// <copyright file="Interfaces.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Globalization;

namespace SecureSBClient.Interfaces
{
    internal interface IQueueMessageSender
    {
        bool CreateClient(string sbNamespace, string? clientId = null);

        Task<bool> SendMessageAsync(string queue, string message);
    }
}
