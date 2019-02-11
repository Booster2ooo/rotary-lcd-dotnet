using System;
using System.Device.Gpio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RotaryLcd.Services
{
    
    public class GpioControllerService
        : IDisposable 
    {

        public GpioControllerService(
            ILogger<ProgramService> logger,
            IOptions<RotaryLcdConfig> config
        ) {
            this.Logger = logger;
            this.Config = config.Value;
            this.Controller = new GpioController(this.Config.PinNumberingScheme);
        }

        public readonly GpioController Controller;
        private readonly ILogger Logger;
        private readonly RotaryLcdConfig Config;
        
        public void Dispose()
        {
            this.Logger.LogInformation("Disposing GpioControllerService....");
            this.Controller.Dispose();
        }
    }
}