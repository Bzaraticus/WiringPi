using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WiringPi.Devices
{
    public class MCP23017 : PinController
    {
        // Port extender, connected with I2C, interrupts connected

        private int DeviceID;
        private int FD;
        private List<KeyValuePair<int, Wrapper.ISRCallback>> InterruptCB = new List<KeyValuePair<int, Wrapper.ISRCallback>>();
        private Dictionary<int, InterruptMode> InterruptModes = new Dictionary<int, InterruptMode>();
        private int[] PinStates = new int[16];

        private int ADDR_IODIR = 0x00;
        private int ADDR_GPPU = 0x0C;
        private int ADDR_GPIO = 0x12;
        private int ADDR_OLAT = 0x14;

        public MCP23017(int devid)
        {
            DeviceID = devid;

            FD = Wrapper.wiringPiI2CSetup(DeviceID);

            int GPIO = Wrapper.wiringPiI2CReadReg16(FD, ADDR_GPIO);
            for (int i = 0; i < 16; i++)
            {
                PinStates[i] = GetRegisterBit(GPIO, i);
            }

            new Thread(ReadThread).Start();
        }

        private void ReadThread()
        {
            while (true)
            {
                int GPIO = Wrapper.wiringPiI2CReadReg16(FD, ADDR_GPIO);
                for (int i = 0; i < 16; i++)
                {
                    int nval = GetRegisterBit(GPIO, i);
                    if (nval != PinStates[i])
                    {
                        PinStates[i] = nval;

                        if (InterruptModes.ContainsKey(i))
                        {
                            InterruptMode mode = InterruptModes[i];
                            if (mode == InterruptMode.Both || (mode == InterruptMode.RisingEdge && nval == 1) || (mode == InterruptMode.FallingEdge && nval == 0))
                            {
                                foreach (KeyValuePair<int, Wrapper.ISRCallback> kv in InterruptCB)
                                {
                                    if (kv.Key == i)
                                    {
                                        kv.Value();
                                    }
                                }
                            }
                        }
                    }
                }
                Wrapper.delayMicroseconds(1);
            }
        }

        public ExtendedPin GetPin(int num)
        {
            return new ExtendedPin(this, num);
        }

        private void WriteRegisterBit(int addr, int bit, int val)
        {
            int reg = Wrapper.wiringPiI2CReadReg16(FD, addr);
            reg = SetRegisterBit(reg, bit, val);
            Wrapper.wiringPiI2CWriteReg16(FD, addr, reg);
        }

        private int SetRegisterBit(int reg, int bit, int val)
        {
            int bval = 1 << bit;
            int ret = reg;
            if (val == 0)
            {
                ret &= ~bval;
            }
            else
            {
                ret |= bval;
            }
            return ret;
        }

        private int ReadRegisterBit(int addr, int bit)
        {
            int reg = Wrapper.wiringPiI2CReadReg16(FD, addr);
            return GetRegisterBit(reg, bit);
        }

        private int GetRegisterBit(int reg, int bit)
        {
            int bval = 1 << bit;          
            return (reg & bval) == bval ? 1 : 0;
        }

        public override void SetIOMode(int pin, PinMode mode)
        {
           if (mode == PinMode.Input)
           {
               WriteRegisterBit(ADDR_IODIR, pin, 1);
           }
           else if (mode == PinMode.Output)
           {
               WriteRegisterBit(ADDR_IODIR, pin, 0);
           }
        }

        public override void PullUpDown(int pin, PullUpDownMode pud)
        {
            if (pud == PullUpDownMode.Off)
            {
                WriteRegisterBit(ADDR_GPPU, pin, 0);
            }
            else if (pud == PullUpDownMode.Up)
            {
                WriteRegisterBit(ADDR_GPPU, pin, 1);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override void DigitalWrite(int pin, int val)
        {
            WriteRegisterBit(ADDR_OLAT, pin, val);
        }

        public override int DigitalRead(int pin)
        {
            return PinStates[pin];
        }

        public override void SetupInterrupt(int pin, InterruptMode mode)
        {
            InterruptModes[pin] = mode;
        }

        public override void AddInterruptCallback(int pin, Wrapper.ISRCallback cb)
        {
            InterruptCB.Add(new KeyValuePair<int, Wrapper.ISRCallback>(pin, cb));
        }
    }
}
