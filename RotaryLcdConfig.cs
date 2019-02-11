using System;
using System.Device.Gpio;

namespace RotaryLcd
{
    public class RotaryLcdConfig 
    {

        public RotaryLcdConfig() {
            this.RotaryEncoderPins = new RotaryEncoderPinsOptions();
            this.LcdPins = new LcdPinsOptions();
        }
        
        public PinNumberingScheme PinNumberingScheme { get; set; } 
        public RotaryEncoderPinsOptions RotaryEncoderPins { get; set; }
        public LcdPinsOptions LcdPins { get; set; }
        
        public class RotaryEncoderPinsOptions {
            public int Switch { get; set; } 
            public int Clock { get; set; } 
            public int Data { get; set; } 
        }

        public class LcdPinsOptions {
            public int Reset { get; set; } 
            public int Enable { get; set; } 
            public int Data4 { get; set; } 
            public int Data5 { get; set; } 
            public int Data6 { get; set; } 
            public int Data7 { get; set; } 
            public int Backlight { get; set; }
        }   
    }
}

