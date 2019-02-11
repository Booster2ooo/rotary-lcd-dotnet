using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RotaryLcd.Services;

namespace RotaryLcd
{
    class Program
    {
        static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler;
            IHostBuilder builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    IHostingEnvironment env = hostingContext.HostingEnvironment;
                    config
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
					services.AddOptions();
                    services.Configure<RotaryLcdConfig>(hostContext.Configuration.GetSection("RotaryLcd"));
                    services.AddSingleton<GpioControllerService>();
                    services.AddSingleton<LcdService>();
                    services.AddSingleton<RotaryEncoderService>();
                    services.AddSingleton<RotaryDisplayService>();
                    services.AddSingleton<IHostedService, ProgramService>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync(CancellationTokenSource.Token);
        }

        private static void ProcessExitHandler(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel();
        }
    }
}
