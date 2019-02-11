using System;

namespace RotaryLcd
{
    public class RotaryChangeEventArgs: EventArgs
    {
        public RotaryChangeEventType RotaryChangeEventType { get; }

        public RotaryChangeEventArgs(RotaryChangeEventType rotaryChangeEventType) {
            this.RotaryChangeEventType = rotaryChangeEventType;
        }
    }
}