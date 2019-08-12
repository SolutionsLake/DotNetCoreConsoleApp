using System;
using System.IO;
using System.Threading.Tasks;
using ConsoleDemo.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace ConsoleDemo.App
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            const string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Information, loggerTemplate, theme: AnsiConsoleTheme.Literate)
                .WriteTo.File("./App_Data/logs/log.txt", restrictedToMinimumLevel: LogEventLevel.Information, loggerTemplate,
                    rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
                .CreateLogger();

            try
            {
                Log.Information("====================================================================");
                Log.Information($"Application Starts. Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}");

                var host = new HostBuilder()
                    .ConfigureHostConfiguration(configHost =>
                    {
                        configHost.SetBasePath(Directory.GetCurrentDirectory());
                        configHost.AddJsonFile("config/hostsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"config/hostsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true);
                        configHost.AddEnvironmentVariables(prefix: "ConsoleDemo_");
                        configHost.AddCommandLine(args);
                    })
                    .ConfigureAppConfiguration((hostContext, configApp) =>
                    {
                        Log.Information($"Hosting Environment: {hostContext.HostingEnvironment.EnvironmentName}; Hosting Machine: {Environment.MachineName}");
                        configApp.AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"config/appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"config/appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddSingleton(args);
                        services.AddHostedService<WorkerService>();
                        services.AddScoped<IProcessor, Processor>();
                    })
                    .UseConsoleLifetime()
                    .UseSerilog()
                    .Build();

                await host.RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.Information("====================================================================\r\n");
                Log.CloseAndFlush();
            }

        }
    }
}
