using ConsoleApp.Classes;
using ConsoleApp.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        private static IMainBootstrapper? _mainBootstrapper;

        private static void Main(string[] args)
        {
            #region Initialization
            // Configuration: Managed by Host.CreateDefaultBuilder
            var host = CreateHostBuilder(args).Build();
            #endregion

            _mainBootstrapper = host.Services.GetRequiredService<IMainBootstrapper>();
            _mainBootstrapper.Run(args);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            // Ref: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-7.0
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddSingleton<IMainBootstrapper, MainBootstrapper>()
                        .AddSingleton<IConsoleWrapper, ConsoleWrapper>();
                });
    }
}