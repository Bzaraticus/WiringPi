using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WiringPi.Devices
{
    public class Switch
    {
        public delegate void SwitchChangeEvent(int nval);

        private DigitalPin Pin;
        private int Debounce = 10;
        private Stopwatch Timer = new Stopwatch();
        private int Value;

        public SwitchChangeEvent OnChange;

        public Switch(DigitalPin pin, PullUpDownMode pud)
        {
            Pin = pin;

            Pin.PullUpDown(pud);
            Pin.SetIOMode(PinMode.Input);
            Pin.SetupInterrupt(InterruptMode.Both);
            Pin.AddInterruptCallback(InterruptCB);

            Value = pin.DigitalRead();

            Timer.Start();
        }

        private void InterruptCB()
        {
            int nval = Pin.DigitalRead();
            if (nval != Value && Timer.Elapsed.TotalMilliseconds > Debounce)
            {
                Value = nval;
                if (OnChange != null)
                {
                    OnChange(nval);
                }
                Timer.Restart();
            }
        }

        private int GetState()
        {
            return Value;
        }
    }
}
