using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiringPi.Devices
{
    public abstract class PinController
    {
        public abstract void SetIOMode(int pin, PinMode mode);
        public abstract void PullUpDown(int pin, PullUpDownMode pud);
        public abstract void DigitalWrite(int pin, int val);
        public abstract int DigitalRead(int pin);
        public abstract void SetupInterrupt(int pin, InterruptMode mode);
        public abstract void AddInterruptCallback(int pin, Wrapper.ISRCallback cb);
    }
}
