// <copyright file="IDnsResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ClientConsoleApp.Interfaces
{
    internal interface IDnsResolver
    {
        Task<bool> ResolveAsync(string fqdn);
    }
}
