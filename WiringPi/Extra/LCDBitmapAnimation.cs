using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace WiringPi.Extra
{
    public class LCDBitmapAnimation
    {
        public int FrameWidth;
        public int FrameHeight;
        public List<LCDBitmap> Frames = new List<LCDBitmap>();
        public List<int> FrameTimes = new List<int>();
        public int Length;
        public float CurrentTime = 0;
        public bool Looping = true;

        public static LCDBitmapAnimation Load(Bitmap img)
        {
            LCDBitmapAnimation anim = new LCDBitmapAnimation();
            anim.FrameHeight = img.Height;
            anim.FrameWidth = img.Width;

            int fcount = img.GetFrameCount(FrameDimension.Time);
            if (fcount == 1)
            {
                anim.Frames.Add(LCDBitmap.Load(img));
                anim.FrameTimes.Add(1);
            }
            else
            {
                byte[] times = img.GetPropertyItem(0x5100).Value;
                for (int i = 0; i < fcount; i++)
                {
                    img.SelectActiveFrame(FrameDimension.Time, i);
                    int dur = BitConverter.ToInt32(times, (i * 4) % times.Length) * 10;
                    anim.FrameTimes.Add(dur);
                    anim.Frames.Add(LCDBitmap.Load(img));
                }
            }
            
            anim.Length = anim.FrameTimes.Sum();

            return anim;
        }

        public LCDBitmapAnimation GetSequence(int start, int count)
        {
            LCDBitmapAnimation anim = new LCDBitmapAnimation();
            anim.FrameHeight = FrameHeight;
            anim.FrameWidth = FrameWidth;
            anim.Frames.AddRange(Frames.GetRange(start, count));
            anim.FrameTimes.AddRange(FrameTimes.GetRange(start, count));
            anim.Length = anim.FrameTimes.Sum();
            return anim;
        }

        public LCDBitmapAnimation Copy()
        {
            return GetSequence(0, Frames.Count);
        }

        public LCDBitmap GetFrame(int n)
        {
            return Frames[n];
        }

        public LCDBitmap GetFrameAtTime(float time)
        {
            float tm = time;
            if (Looping)
            {
                tm %= Length;
            }
            else if (tm > Length)
            {
                tm = 0;
            }

            int curtime = 0;
            for (int i = 0; i < Frames.Count; i++)
            {
                curtime += FrameTimes[i];
                if (tm <= curtime)
                {
                    return Frames[i];
                }
            }

            return null;
        }

        public LCDBitmap GetCurrentFrame()
        {
            return GetFrameAtTime(CurrentTime);
        }
    }
}
