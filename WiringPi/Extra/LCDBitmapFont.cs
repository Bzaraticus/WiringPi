using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WiringPi.Extra
{
    public class LCDBitmapFont
    {
        public string Name;
        public Dictionary<string, LCDBitmap> Characters = new Dictionary<string,LCDBitmap>();

        private static Dictionary<string, LCDBitmapFont> Fonts = new Dictionary<string, LCDBitmapFont>();

        public LCDBitmapFont(string name, string file, string[][] charmap)
        {
            Name = name;
            if (Fonts.ContainsKey(name))
            {
                Characters = Fonts[name].Characters;
            }
            else
            {
                Bitmap bmp = new Bitmap(file);

                int ofsy = 0;
                for (int chary = 0; chary < charmap.Length; chary++)
                {
                    int lineheight = 0;
                    for (int liney = ofsy; liney < bmp.Height; liney++)
                    {
                        Color col = bmp.GetPixel(0, liney);
                        if (col.R == 0 && col.G == 255 && col.B == 0)
                        {
                            break;
                        } 
                        else
                        {
                            lineheight++;
                        }
                    }

                    int ofsx = 0;
                    for (int charx = 0; charx < charmap[chary].Length; charx++)
                    {
                        int charwidth = 0;
                        for (int posx = ofsx; posx < bmp.Width; posx++)
                        {
                            Color col = bmp.GetPixel(posx, ofsy);
                            if (col.R == 0 && col.G == 255 && col.B == 0)
                            {
                                break;
                            }
                            else
                            {
                                charwidth++;
                            }
                        }

                        LCDBitmap charbmp = new LCDBitmap(charwidth, lineheight);
                        for (int posy = 0; posy < lineheight; posy++)
                        {
                            for (int posx = 0; posx < charwidth; posx++)
                            {
                                Color col = bmp.GetPixel(ofsx + posx, ofsy + posy);
                                charbmp.SetPixel(posx, posy, (col.R == 0 && col.G == 0 && col.B == 0) ? 1 : 0);
                            }
                        }
                        Characters.Add(charmap[chary][charx], charbmp);

                        ofsx += charwidth + 1;
                    }

                    ofsy += lineheight + 1;
                }

                Fonts[name] = this;
            }
        }
    }
}
