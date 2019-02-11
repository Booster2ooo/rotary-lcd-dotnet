using System;
using System.Device.Gpio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RotaryLcd.Services
{
    public class RotaryEncoderService
        : IDisposable
    {

        public RotaryEncoderService(
            ILogger<ProgramService> logger,
            IOptions<RotaryLcdConfig> config,
            GpioControllerService controllerService
        ) {
            this.Logger = logger;
            this.Config = config.Value;
            this.Controller = controllerService.Controller;
            this.SwitchPin = this.Config.RotaryEncoderPins.Switch;
            this.ClockPin = this.Config.RotaryEncoderPins.Clock;
            this.DataPin = this.Config.RotaryEncoderPins.Data;
            this.SwitchState = PinValue.High;
            this.ClockState = PinValue.High;
            this.Controller.OpenPin(this.SwitchPin, PinMode.InputPullUp);
            this.Controller.OpenPin(this.ClockPin, PinMode.InputPullUp);
            this.Controller.OpenPin(this.DataPin, PinMode.InputPullUp);
            this.Controller.RegisterCallbackForPinValueChangedEvent(this.SwitchPin, PinEventTypes.Falling, this.OnSwitchFalling);
            this.Controller.RegisterCallbackForPinValueChangedEvent(this.SwitchPin, PinEventTypes.Rising, this.OnSwitchRising);
            this.Controller.RegisterCallbackForPinValueChangedEvent(this.ClockPin, PinEventTypes.Falling, this.OnRotaryFalling);
            this.Controller.RegisterCallbackForPinValueChangedEvent(this.ClockPin, PinEventTypes.Rising, this.OnRotaryRising);
        }


        private readonly ILogger Logger;
        private readonly RotaryLcdConfig Config;
        private readonly GpioController Controller;
        private readonly int SwitchPin;
        private readonly int ClockPin;
        private readonly int DataPin;
        private PinValue SwitchState { get; set; } 
        private PinValue ClockState { get; set; }

        private void OnChanged(RotaryChangeEventArgs e) 
        {
            RotaryChangedHandler handler = this.RotaryChangedEvent;
            if (handler != null) {
                handler(this, e);
            }
        }

        public delegate void RotaryChangedHandler(Object sender, RotaryChangeEventArgs e);
        public event RotaryChangedHandler RotaryChangedEvent;

        private void OnSwitchFalling(Object sender, PinValueChangedEventArgs pinValueChangedEventArgs) 
        {
            this.HandleSwitchChange(pinValueChangedEventArgs);
        }
        private void OnSwitchRising(Object sender, PinValueChangedEventArgs pinValueChangedEventArgs) 
        {
            this.HandleSwitchChange(pinValueChangedEventArgs);
        }
        private void HandleSwitchChange(PinValueChangedEventArgs pinValueChangedEventArgs) 
        {
            this.Logger.LogInformation("Switch event: " + pinValueChangedEventArgs.ChangeType);
            PinValue switchState = this.Controller.Read(pinValueChangedEventArgs.PinNumber);
            if (switchState != this.SwitchState) {
                this.SwitchState = switchState;
                if (this.SwitchState == PinValue.High) {
                    this.Logger.LogInformation("Switch released");
                    this.OnChanged(new RotaryChangeEventArgs(RotaryChangeEventType.SwitchReleased));
                }
                else {
                    this.Logger.LogInformation("Switch pressed");
                    this.OnChanged(new RotaryChangeEventArgs(RotaryChangeEventType.SwitchPressed));
                }
            }
        }
        
        private void OnRotaryFalling(Object sender, PinValueChangedEventArgs pinValueChangedEventArgs) 
        {
            this.HandleRotaryChange(pinValueChangedEventArgs);
        }
        private void OnRotaryRising(Object sender, PinValueChangedEventArgs pinValueChangedEventArgs) 
        {
            this.HandleRotaryChange(pinValueChangedEventArgs);
        }
        private void HandleRotaryChange(PinValueChangedEventArgs pinValueChangedEventArgs) 
        {
            this.Logger.LogInformation("Rotary event: " + pinValueChangedEventArgs.ChangeType);
            PinValue clockState = this.Controller.Read(pinValueChangedEventArgs.PinNumber);
            PinValue dataState = this.Controller.Read(this.DataPin);
            if (clockState != this.ClockState) {
                this.ClockState = clockState;
                if (this.ClockState == PinValue.High) {
                    if (dataState == PinValue.High) {
                        this.Logger.LogInformation("Rotary direction >>");
                        this.OnChanged(new RotaryChangeEventArgs(RotaryChangeEventType.TurnedRight));
                    }
                    else {
                        this.Logger.LogInformation("Rotary direction <<");
                        this.OnChanged(new RotaryChangeEventArgs(RotaryChangeEventType.TurnedLeft));
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Logger.LogInformation("Disposing RotaryEncoderService...");
            this.Controller.UnregisterCallbackForPinValueChangedEvent(this.SwitchPin, OnSwitchFalling);
            this.Controller.UnregisterCallbackForPinValueChangedEvent(this.SwitchPin, OnSwitchRising);
            this.Controller.UnregisterCallbackForPinValueChangedEvent(this.ClockPin, OnRotaryFalling);
            this.Controller.UnregisterCallbackForPinValueChangedEvent(this.ClockPin, OnRotaryRising);
        }
        
    }
}