// <copyright file="IDnsResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ClientConsoleAppV2.Interfaces
{
    internal interface IDnsResolver
    {
        Task<bool> ResolveAsync(string fqdn);
    }
}
