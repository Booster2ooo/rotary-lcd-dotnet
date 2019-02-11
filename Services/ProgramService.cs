using System;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.CharacterLcd;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
	
namespace RotaryLcd.Services
{

    public class ProgramService 
        : IHostedService, IDisposable
    {
        public ProgramService(
            ILogger<ProgramService> logger, 
            IOptions<RotaryLcdConfig> config,
            RotaryEncoderService rotaryEncoder,
            RotaryDisplayService rotaryDisplay
        ) {
            this.Logger = logger;
            this.Config = config.Value;
            this.RotaryEncoder = rotaryEncoder;
            this.RotaryDisplay = rotaryDisplay;
        }

        private readonly ILogger Logger;
        private readonly RotaryLcdConfig Config;
        private readonly RotaryEncoderService RotaryEncoder;
        private readonly RotaryDisplayService RotaryDisplay;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Starting program");
            this.RotaryEncoder.RotaryChangedEvent += (Object sender, RotaryChangeEventArgs e) => 
            { 
                if (e.RotaryChangeEventType == RotaryChangeEventType.SwitchReleased) Environment.Exit(0); 
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Stopping program.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Logger.LogInformation("Disposing program....");
        }
    }
}
