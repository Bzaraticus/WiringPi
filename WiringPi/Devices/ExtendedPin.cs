using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiringPi.Devices
{
    public class ExtendedPin : DigitalPin
    {
        private int PinNum;
        private PinController Controller;

        public ExtendedPin(PinController controller, int num)
        {
            PinNum = num;
            Controller = controller;
        }

        public override void SetIOMode(PinMode mode)
        {
            Controller.SetIOMode(PinNum, mode);
        }

        public override void PullUpDown(PullUpDownMode pud)
        {
            Controller.PullUpDown(PinNum, pud);
        }

        public override void DigitalWrite(int val)
        {
            Controller.DigitalWrite(PinNum, val);
        }

        public override int DigitalRead()
        {
            return Controller.DigitalRead(PinNum);
        }

        public override void SetupInterrupt(InterruptMode mode)
        {
            Controller.SetupInterrupt(PinNum, mode);
        }

        public override void AddInterruptCallback(Wrapper.ISRCallback cb)
        {
            Controller.AddInterruptCallback(PinNum, cb);
        }
    }
}
