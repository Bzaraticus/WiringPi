using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiringPi
{
    public enum GPIOPinMode
    {
        Input = Wrapper.INPUT, Output = Wrapper.OUTPUT, PWMOutput = Wrapper.PWM_OUTPUT, GPIOClock = Wrapper.GPIO_CLOCK
    }
}
