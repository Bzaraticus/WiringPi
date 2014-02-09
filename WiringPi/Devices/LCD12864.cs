using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WiringPi.Extra;

namespace WiringPi.Devices
{
    public class LCD12864
    {
        private DigitalPin EN;
        private DigitalPin RS;
        private DigitalPin RW;
        private DigitalPin RST;
        private DigitalPin[] DB;

        private LCDBitmap Buffer;

        public LCD12864(DigitalPin en, DigitalPin rs, DigitalPin rw, DigitalPin rst, DigitalPin[] d)
        {
            EN = en;
            RS = rs;
            RW = rw;
            RST = rst;
            DB = d;

            EN.SetIOMode(PinMode.Output);
            EN.DigitalWrite(0);
            RS.SetIOMode(PinMode.Output);
            RW.SetIOMode(PinMode.Output);

            for (int i = 0; i < 8; i++)
            {
                DB[i].SetIOMode(PinMode.Output);
            }

            RST.SetIOMode(PinMode.Output);
            RST.DigitalWrite(0);
            Wrapper.delayMicroseconds(10);
            RST.DigitalWrite(1);
            Wrapper.delay(100);

            WriteInstruction(Convert.ToInt32("00110000", 2)); // Function set
            WriteInstruction(Convert.ToInt32("00110000", 2)); // Function set
            WriteInstruction(Convert.ToInt32("00001100", 2)); // Display on/off
            WriteInstruction(Convert.ToInt32("00000001", 2)); // Display clear
            WriteInstruction(Convert.ToInt32("00000110", 2)); // Entry mode set
            WriteInstruction(Convert.ToInt32("00110100", 2)); // Extended Mode
            WriteInstruction(Convert.ToInt32("00110110", 2)); // Graphics mode

            // Screen clear
            Buffer = new LCDBitmap(128, 64, 1);
            Draw(new LCDBitmap(128, 64, 0));
        }

        private int GetBufferData(int[,] buff, int posx, int posy)
        {
            return buff[posx, posy] * 128 + 
                buff[posx + 1, posy] * 64 + 
                buff[posx + 2, posy] * 32 +
                buff[posx + 3, posy] * 16 + 
                buff[posx + 4, posy] * 8 + 
                buff[posx + 5, posy] * 4 + 
                buff[posx + 6, posy] * 2 + 
                buff[posx + 7, posy];
        }

        public void Draw(LCDBitmap bmp)
        {
            int[,] oldbuffer = Buffer.GetBuffer();
            int[,] buffer = bmp.GetBuffer();

            // Compute changes, draw whats changed
            for (int memy = 0; memy < 32; memy++)
            {
                int[][] data = new int[16][];
                int firstchanged = -1;
                int lastchanged = -1;
                for (int memx = 0; memx < 16; memx++)
                {
                    int posx = memx * 16;
                    int posy = memy;
                    if (posx >= 128)
                    {
                        posy += 32;
                        posx -= 128;
                    }

                    int dold1 = GetBufferData(oldbuffer, posx, posy);
                    int dold2 = GetBufferData(oldbuffer, posx + 8, posy);
                    int dnew1 = GetBufferData(buffer, posx, posy);
                    int dnew2 = GetBufferData(buffer, posx + 8, posy);

                    data[memx] = new int[] { dnew1, dnew2 };
                    if (dnew1 != dold1 || dnew2 != dold2)
                    {            
                        if (firstchanged == -1)
                        {
                            firstchanged = memx;
                        }
                        lastchanged = memx;
                    }
                }

                // Draw
                if (firstchanged != -1)
                {
                    WriteInstruction(128 + memy);
                    WriteInstruction(128 + firstchanged);
                    for (int memx = firstchanged; memx <= lastchanged; memx++)
                    {
                        WriteData(data[memx][0]);
                        WriteData(data[memx][1]);
                    }
                }
            }

            Buffer.DrawLCDBitmap(bmp, 0, 0, 0, 0, 128, 64);
        }

        public void DebugData(int data)
        {
            for (int i = 7; i >= 0; i--)
            {
                int val = ((data & (1 << i))) > 0 ? 1 : 0;
                Console.Write(val.ToString() + ", ");
            }
            Console.WriteLine();
        }

        private void Wait()
        {
            while (true)
            {
                int rd = Read(0);
                if ((rd & 128) == 0)
                {
                    break;
                }
            }
        }

        private void WriteInstruction(int d)
        {
            Write(0, d);
        }

        private void WriteData(int d)
        {
            Write(1, d);
        }

        private void Write(int rs, int d)
        {
            //Console.Write("Write (" + rs.ToString() + "): ");
            //DebugData(d);

            for (int i = 0; i < 8; i++)
            {
                DB[i].SetIOMode(PinMode.Output);
            }

            RW.DigitalWrite(0);
            RS.DigitalWrite(rs);   
            for (int i = 0; i < 8; i++)
            {
                DB[i].DigitalWrite(((d & (1 << i))) > 0 ? 1 : 0);
            }

            Wrapper.delayMicroseconds(1);
            EN.DigitalWrite(1);
            Wrapper.delayMicroseconds(1);
            EN.DigitalWrite(0);

            Wait();
            //Wrapper.delayMicroseconds(100);
            //Wrapper.delay(1);

            //Console.Write("Read: ");
            //DebugData(Read(0));
        }

        private int Read(int rs)
        {
            for (int i = 0; i < 8; i++)
            {
                DB[i].SetIOMode(PinMode.Input);
                DB[i].PullUpDown(PullUpDownMode.Up);
            }

            RW.DigitalWrite(1);
            RS.DigitalWrite(rs);
            EN.DigitalWrite(1);
            Wrapper.delayMicroseconds(1);

            int d = 0;
            for (int i = 0; i < 8; i++)
            {
                d += DB[i].DigitalRead() * (1 << i);
            }

            EN.DigitalWrite(0);

            return d;
        }
    }
}
