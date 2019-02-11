using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.CharacterLcd;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RotaryLcd.Services
{
    public class RotaryDisplayService
        : IDisposable
    {

        public RotaryDisplayService(
            ILogger<ProgramService> logger,
            IOptions<RotaryLcdConfig> config,
            RotaryEncoderService rotaryEncoder,
            LcdService lcdService
        ) {
            this.Logger = logger;
            this.Config = config.Value;
            this.RotaryEncoder = rotaryEncoder;
            this.Lcd = lcdService.Lcd;
            this.RotaryEncoder.RotaryChangedEvent += this.OnRotaryChanged;
            this.Write();
        }

        private readonly ILogger Logger;
        private readonly RotaryLcdConfig Config;
        private readonly RotaryEncoderService RotaryEncoder;
        private readonly Lcd1602 Lcd;
        private Task WaitingTask { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private CancellationToken CancellationToken { get; set; }

        private void CancelTask() {
            this.CancellationTokenSource.Cancel();
            this.WaitingTask = null;
        }

        private void OnRotaryChanged(Object sender, RotaryChangeEventArgs e)
        {
            if (this.WaitingTask != null) {
                this.CancelTask();
            }
            switch(e.RotaryChangeEventType) 
            {
                case RotaryChangeEventType.SwitchPressed:
                    this.Write("Pressed");
                    break;
                case RotaryChangeEventType.SwitchReleased:
                    this.Write("Released");
                    break;
                case RotaryChangeEventType.TurnedLeft:
                    this.Write("Previous");
                    break;
                case RotaryChangeEventType.TurnedRight:
                    this.Write("Next");
                    break;
                default:
                    this.Write();
                    break;
            }
            this.CancellationTokenSource = new CancellationTokenSource();
            this.CancellationToken = this.CancellationTokenSource.Token;
            this.WaitingTask = Task.Delay(new TimeSpan(0,0,1)).ContinueWith(c => {
                this.Write();
                this.CancelTask();
            }, this.CancellationToken);
        }

        private void Write(String message = null) 
        {
            try {
            this.Lcd.Clear();
            this.Lcd.Home();
            if (String.IsNullOrEmpty(message)) {
                this.Lcd.Write("Waiting...");
            }
            else {
                this.Lcd.Write("Encoder input:");
                this.Lcd.SetCursorPosition(0,1);
                this.Lcd.Write(message);
            }
            }
            catch (Exception ex) {
                this.Logger.LogDebug("Exception in RotaryDisplayService.Write: " + ex.Message);
            }
        }
        
        public void Dispose()
        {
            this.Logger.LogInformation("Disposing RotaryDisplayService...");
            this.Lcd.Clear();
        }

    }
}