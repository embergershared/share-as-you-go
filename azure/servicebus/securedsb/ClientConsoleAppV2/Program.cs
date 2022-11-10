using ClientConsoleAppV2.Classes;
using ClientConsoleAppV2.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClientConsoleAppV2
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            #region Initialization
            // Configuration: Managed by Host.CreateDefaultBuilder
            var host = CreateHostBuilder(args).Build();
            #endregion

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Program started");
            
            var consoleExecuteInstance = host.Services.GetRequiredService<IConsoleExecute>();
            await consoleExecuteInstance.ExecuteAsync();

            logger.LogInformation("Program finished");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            // Ref: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-7.0
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddTransient<IConsoleExecute, ConsoleExecute>()
                        .AddSingleton<IDnsResolver, DnsResolver>()
                        .AddSingleton<IServiceBusClient, ServiceBusClient>();
                })
                .ConfigureLogging((_, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(options =>
                    {
                        // Ref: https://learn.microsoft.com/en-us/dotnet/core/extensions/console-log-formatter
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "yyy-MM-dd HH:mm:ss.fff ";
                    });
                });
    }
}