using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace WiringPi.Extra
{
    public class LCDBitmap
    {
        public int Width;
        public int Height;

        private int[,] Buffer;

        public LCDBitmap(int width, int height, int bg = 0)
        {
            Width = width;
            Height = height;

            Buffer = new int[Width, Height];
            Clear(bg);
        }

        public static unsafe LCDBitmap Load(Bitmap bmp)
        {
            LCDBitmap lcdbmp = new LCDBitmap(bmp.Width, bmp.Height);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int bpp = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
            byte* fpix = (byte*)data.Scan0;

            Parallel.For(0, bmp.Height, py =>
            {
                byte* line = fpix + (py * data.Stride);
                for (int px = 0; px < bmp.Width; px++)
                {
                    byte b = 255, g = 255, r = 255, a = 0;
                 
                    if (bmp.Palette.Entries.Length > 0)
                    {
                        Color col = bmp.Palette.Entries[line[px * bpp]];      
                        b = col.B;
                        g = col.G;
                        r = col.R;
                        a = col.A;
                    }
                    else
                    {
                        if (bpp == 1)
                        {
                            b = g = r = line[px * bpp];
                        }
                        else if (bpp == 3 || bpp == 4)
                        {
                            b = line[px * bpp];
                            g = line[(px + 1) * bpp];
                            r = line[(px + 2) * bpp];
                            if (bpp == 4)
                            {
                                a = line[(px + 3) * bpp];
                            }
                        }
                    } 

                    if (a == 0)
                    {
                         lcdbmp.Buffer[px, py] = -1;
                    }
                    else if (r == 0 && g == 0 && b == 0)
                    {
                        lcdbmp.Buffer[px, py] = 1;
                    }
                    else
                    {
                        lcdbmp.Buffer[px, py] = 0;
                    }
                }
            });

            bmp.UnlockBits(data);

            return lcdbmp;
        }

        public int[,] GetBuffer()
        {
            return Buffer;
        }

        public void SetPixel(int px, int py, int val)
        {
            if (px >= 0 && px < Width && py >= 0 && py < Height)
            {
                Buffer[px, py] = val;
            }
        }

        public int GetPixel(int px, int py, int def = 0)
        {
            if (px >= 0 && px < Width && py >= 0 && py < Height)
            {
                return Buffer[px, py];
            }
            else
            {
                return -1;
            }
        }

        public void FillRect(int bg, int posx, int posy, int sx, int sy)
        {
            for (int x = posx; x < posx + sx; x++)
            {
                for (int y = posy; y < posy + sy; y++)
                {
                    SetPixel(x, y, bg);
                }
            }
        }

        public void Clear(int bg)
        {
            FillRect(bg, 0, 0, Width, Height);
        }

        public void DrawLCDBitmap(LCDBitmap src, int srcposx, int srcposy, int destposx, int destposy, int sx, int sy)
        {
            for (int x = 0; x < sx; x++)
            {
                for (int y = 0; y < sy; y++)
                {
                    int val = src.GetPixel(srcposx + x, srcposy + y);
                    if (val != -1)
                    {
                        SetPixel(destposx + x, destposy + y, val);
                    }
                }
            }
        }

        public void DrawString(int px, int py, LCDBitmapFont font, string txt, int spacing = 1)
        {
            int posx = px;
            for (int i = 0; i < txt.Length; i++)
            {
                string chr = txt.Substring(i, 1);

                LCDBitmap charbmp = font.Characters[chr];
                DrawLCDBitmap(charbmp, 0, 0, posx, py, charbmp.Width, charbmp.Height);
                posx += charbmp.Width + spacing;
            }
        }

        public static int MeasureString(LCDBitmapFont font, string txt, int spacing = 1)
        {
            int width = 0;
            for (int i = 0; i < txt.Length; i++)
            {
                string chr = txt.Substring(i, 1);

                LCDBitmap charbmp = font.Characters[chr];
                width += charbmp.Width + spacing;
            }
            return width;
        }

        public void DrawLine(int bg, int x1, int y1, int x2, int y2)
        {
            int dx = (int)Math.Abs(x2 - x1);
            int dy = (int)Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;

            int x = x1;
            int y = y1;
            while (true)
            {
                SetPixel(x, y, bg);
                if (x == x2 && y == y2)
                {
                    break;
                }
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (x == x2 && y == y2)
                {
                    SetPixel(x, y, bg);
                    break;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }

        public void DrawPoly(int bg, List<int[]> pts)
        {
            for (int i = 0; i < pts.Count; i++)
            {
                int[] p1 = pts[i];
                int[] p2 = pts[(i + 1) % pts.Count];
                DrawLine(bg, p1[0], p1[1], p2[0], p2[1]);
            }
        }

        public void DrawRect(int bg, int left, int top, int width, int height)
        {
            List<int[]> pts = new List<int[]>();
            pts.Add(new int[] { left, top });
            pts.Add(new int[] { left + width, top });
            pts.Add(new int[] { left + width, top + height });
            pts.Add(new int[] { left, top + height });
            DrawPoly(bg, pts);
        }
    }
}
