using System;

namespace RotaryLcd
{

    [Flags]
    public enum RotaryChangeEventType
    {
        TurnedLeft = 0,
        TurnedRight = 1,
        SwitchPressed = 2,
        SwitchReleased = 3
    }
}