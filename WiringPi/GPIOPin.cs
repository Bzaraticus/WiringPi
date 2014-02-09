using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WiringPi
{
    public class GPIOPin : DigitalPin
    {
        private int PinNum;
        private List<Wrapper.ISRCallback> InterruptCallbacks = new List<Wrapper.ISRCallback>();

        public GPIOPin(int num)
        {
            PinNum = num;
        }

        public override void SetIOMode(PinMode mode)
        {
            if (mode == PinMode.Input)
            {
                SetMode(GPIOPinMode.Input);
            }
            else if (mode == PinMode.Output)
            {
                SetMode(GPIOPinMode.Output);
            }
        }

        public void SetMode(GPIOPinMode mode)
        {
            Wrapper.pinMode(PinNum, (int)mode);
        }

        public override void PullUpDown(PullUpDownMode pud)
        {
            if (pud == PullUpDownMode.Off)
            {
                Wrapper.pullUpDnControl(PinNum, Wrapper.PUD_OFF);
            }
            else if (pud == PullUpDownMode.Up)
            {
                Wrapper.pullUpDnControl(PinNum, Wrapper.PUD_UP);
            }
            else if (pud == PullUpDownMode.Down)
            {
                Wrapper.pullUpDnControl(PinNum, Wrapper.PUD_DOWN);
            }
        }

        public override void DigitalWrite(int val)
        {
            Wrapper.digitalWrite(PinNum, val);
        }

        public override int DigitalRead()
        {
            return Wrapper.digitalRead(PinNum);
        }

        public override void SetupInterrupt(InterruptMode mode)
        {
            // Really dont want to do it this way, but Wrapper.wiringPiISR derps
            new Thread(() =>
            {
                DigitalRead();
                int state = DigitalRead();

                while (true)
                {
                    int nstate = DigitalRead();
                    if (nstate != state)
                    {
                        if (mode == InterruptMode.Both || (mode == InterruptMode.RisingEdge && nstate == 1) || (mode == InterruptMode.FallingEdge && nstate == 0))
                        {
                            InterruptCallback();
                        }
                        state = nstate;
                    }
                    Wrapper.delayMicroseconds(1);
                }

            }).Start();
        }

        private void InterruptCallback()
        {
            foreach (Wrapper.ISRCallback cb in InterruptCallbacks)
            {
                cb();
            }
        }

        public override void AddInterruptCallback(Wrapper.ISRCallback cb)
        {
            InterruptCallbacks.Add(cb);
        }
    }
}
