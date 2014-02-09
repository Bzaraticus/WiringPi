using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiringPi
{
    public abstract class DigitalPin
    {
        // Basic digital pin
        // Input/Output modes only, pull up/down, read/write, interrupt on input change

        public abstract void SetIOMode(PinMode mode);
        public abstract void PullUpDown(PullUpDownMode pud);
        public abstract void DigitalWrite(int val);
        public abstract int DigitalRead();
        public abstract void SetupInterrupt(InterruptMode mode);
        public abstract void AddInterruptCallback(Wrapper.ISRCallback cb);
    }
}
