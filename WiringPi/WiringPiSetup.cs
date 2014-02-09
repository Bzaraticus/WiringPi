using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiringPi
{
    public static class WiringPiSetup
    {
        public static void Setup()
        {
            Wrapper.wiringPiSetup();
        }

        public static void SetupPhys()
        {
            Wrapper.wiringPiSetupPhys();
        }

        public static void SetupGPIO()
        {
            Wrapper.wiringPiSetupGpio();
        }

        public static void SetupSys()
        {
            Wrapper.wiringPiSetupSys();
        }
    }
}
