using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiringPi.Devices
{
    public class RotaryEncoder
    {
        public delegate void RotaryEncoderTurnEvent(int delta);

        private DigitalPin Pin1;
        private DigitalPin Pin2;

        private int RotationSequence;
        private int LastDelta = 0;
        private int Position = 0;
        private int StepsPerTurn = 4;

        public RotaryEncoderTurnEvent OnStep;
        public RotaryEncoderTurnEvent OnTurn;

        public RotaryEncoder(DigitalPin p1, DigitalPin p2, PullUpDownMode pud)
        {
            Pin1 = p1;
            Pin2 = p2;

            Pin1.SetIOMode(PinMode.Input);
            Pin1.PullUpDown(pud);
            Pin1.SetupInterrupt(InterruptMode.Both);
            Pin1.AddInterruptCallback(UpdateSequence);

            Pin2.SetIOMode(PinMode.Input);
            Pin2.PullUpDown(pud);
            Pin2.SetupInterrupt(InterruptMode.Both);
            Pin2.AddInterruptCallback(UpdateSequence);

            RotationSequence = GetRotationSequence();
        }

        private int GetRotationSequence()
        {
            int state1 = Pin1.DigitalRead();
            int state2 = Pin2.DigitalRead();
            return (state1 ^ state2) | (state2 << 1);
        }

        public int GetDelta()
        {
            int seq = GetRotationSequence();
            if (seq != RotationSequence)
            {
                int delta = (seq - RotationSequence + 4) % 4;
                if (delta == 3)
                {
                    delta = -1;
                }
                else if (delta == 2)
                {
                    delta = (LastDelta < 0) ? -2 : 2;
                }
                LastDelta = delta;
                RotationSequence = seq;

                return delta;
            }
            else
            {
                return 0;
            }
        }

        public int GetTurns(int delta)
        {
            Position += delta;
            int turns = Position / StepsPerTurn;
            Position -= turns * StepsPerTurn;
            return turns;
        }

        private void UpdateSequence()
        {
            int delta = GetDelta();
            if (delta != 0)
            {
                if (OnStep != null)
                {
                    OnStep(delta);
                }

                int turns = GetTurns(delta);
                
                if (turns != 0)
                {
                    if (OnTurn != null)
                    {
                        OnTurn(turns);
                    }
                }
            }
        }
    }
}
