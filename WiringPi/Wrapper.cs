using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace WiringPi
{
    public static class Wrapper
    {
        // Constants

        public const int INPUT = 0;
        public const int OUTPUT = 1;
        public const int PWM_OUTPUT = 2;
        public const int GPIO_CLOCK = 3;

        public const int PUD_OFF = 0;
        public const int PUD_DOWN = 1;
        public const int PUD_UP = 2;

        public const int INT_EDGE_SETUP = 0;
        public const int INT_EDGE_FALLING = 1;
        public const int INT_EDGE_RISING = 2;
        public const int INT_EDGE_BOTH = 3;

        // Delegates

        public delegate void ISRCallback();

        // Setup

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiSetup")]
        public static extern int wiringPiSetup();

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiSetupGpio")]
        public static extern int wiringPiSetupGpio();

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiSetupSys")]
        public static extern int wiringPiSetupSys();

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiSetupPhys")]
        public static extern int wiringPiSetupPhys();

        // GPIO

        [DllImport("libwiringPi.so", EntryPoint = "pinModeAlt")]
        public static extern void pinModeAlt(int pin, int mode);

        [DllImport("libwiringPi.so", EntryPoint = "pinMode")]
        public static extern void pinMode(int pin, int mode);

        [DllImport("libwiringPi.so", EntryPoint = "pullUpDnControl")]
        public static extern void pullUpDnControl(int pin, int pud);

        [DllImport("libwiringPi.so", EntryPoint = "digitalRead")]
        public static extern int digitalRead(int pin);

        [DllImport("libwiringPi.so", EntryPoint = "digitalWrite")]
        public static extern void digitalWrite(int pin, int value);

        [DllImport("libwiringPi.so", EntryPoint = "pwmWrite")]
        public static extern void pwmWrite(int pin, int value);

        [DllImport("libwiringPi.so", EntryPoint = "analogRead")]
        public static extern int analogRead(int pin);

        [DllImport("libwiringPi.so", EntryPoint = "analogWrite")]
        public static extern void analogWrite(int pin, int value);

        [DllImport("libwiringPi.so", EntryPoint = "digitalWriteByte")]
        public static extern void digitalWriteByte(int value);

        [DllImport("libwiringPi.so", EntryPoint = "pwmSetMode")]
        public static extern void pwmSetMode(int mode);

        [DllImport("libwiringPi.so", EntryPoint = "pwmSetRange")]
        public static extern void pwmSetRange(uint range);

        [DllImport("libwiringPi.so", EntryPoint = "pwmSetClock")]
        public static extern void pwmSetClock(int divisor);

        [DllImport("libwiringPi.so", EntryPoint = "gpioClockSet")]
        public static extern void gpioClockSet(int pin, int freq);

        // Timing

        [DllImport("libwiringPi.so", EntryPoint = "delay")]
        public static extern void delay(uint howLong);

        [DllImport("libwiringPi.so", EntryPoint = "delayMicroseconds")]
        public static extern void delayMicroseconds(uint howLong);

        [DllImport("libwiringPi.so", EntryPoint = "millis")]
        public static extern uint millis();

        [DllImport("libwiringPi.so", EntryPoint = "micros")]
        public static extern uint micros();

        // Interrupts

        [DllImport("libwiringPi.so", EntryPoint = "waitForInterrupt")]
        public static extern int waitForInterrupt(int pin, int mS);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiISR")]
        public static extern int wiringPiISR(int pin, int mode, ISRCallback method);

        // Misc

        [DllImport("libwiringPi.so", EntryPoint = "piBoardRev")]
        public static extern int piBoardRev();

        [DllImport("libwiringPi.so", EntryPoint = "wpiPinToGpio")]
        public static extern int wpiPinToGpio(int wpiPin);

        [DllImport("libwiringPi.so", EntryPoint = "physPinToGpio")]
        public static extern int physPinToGpio(int physPin);

        [DllImport("libwiringPi.so", EntryPoint = "setPadDrive")]
        public static extern void setPadDrive(int group, int value);

        [DllImport("libwiringPi.so", EntryPoint = "getAlt")]
        public static extern int getAlt(int pin);

        [DllImport("libwiringPi.so", EntryPoint = "piHiPri")]
        public static extern int piHiPri(int priority);

        // SPI

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiSPIGetFd")]
        public static extern int wiringPiSPIGetFd(int channel);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiSPIDataRW")]
        public static unsafe extern int wiringPiSPIDataRW(int channel, byte* data, int len);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiSPISetup")]
        public static extern int wiringPiSPISetup(int channel, int speed);

        // I2C

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CRead")]
        public static extern int wiringPiI2CRead(int fd);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CReadReg8")]
        public static extern int wiringPiI2CReadReg8(int fd, int reg);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CReadReg16")]
        public static extern int wiringPiI2CReadReg16(int fd, int reg);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CWrite")]
        public static extern int wiringPiI2CWrite(int fd, int data);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CWriteReg8")]
        public static extern int wiringPiI2CWriteReg8(int fd, int reg, int data);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CWriteReg16")]
        public static extern int wiringPiI2CWriteReg16(int fd, int reg, int data);

        [DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CSetup")]
        public static extern int wiringPiI2CSetup(int devId);
    }
}
