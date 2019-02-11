using System;
using System.Device.Gpio;
using Iot.Device.CharacterLcd;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RotaryLcd.Services
{
    
    public class LcdService
        : IDisposable 
    {

        public LcdService(
            ILogger<ProgramService> logger,
            IOptions<RotaryLcdConfig> config
        ) {
            this.Logger = logger;
            this.Config = config.Value;
            this.Lcd = new Lcd1602(
                this.Config.LcdPins.Reset,
                this.Config.LcdPins.Enable,
                new int[] {
                    this.Config.LcdPins.Data4,
                    this.Config.LcdPins.Data5,
                    this.Config.LcdPins.Data6,
                    this.Config.LcdPins.Data7,
                },
                this.Config.LcdPins.Backlight
            );
        }

        public readonly Lcd1602 Lcd;
        private readonly ILogger Logger;
        private readonly RotaryLcdConfig Config;
        
        public void Dispose()
        {
            this.Logger.LogInformation("Disposing LcdService....");
            this.Lcd.Dispose();
        }
    }
}