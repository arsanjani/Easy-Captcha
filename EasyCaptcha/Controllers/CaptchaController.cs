using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCaptcha.Controllers
{
    [AllowAnonymous]
    public class CaptchaController : Controller
    {
        private string _backgroundColor, _forecolor, _type;
        private int _length;
        /// <summary>
        /// Retuns a random Captcha as a bitmap
        /// You can change Captcha length here if you want.
        /// </summary>
        /// <param name="l">Length of the captcha. Default is 5</param>
        /// <param name="bc">Background color of the captcha. Default is Transparent. You can use random value</param>
        /// <param name="fc">Forecolor of the texts. You can use random value</param>
        /// <param name="t">mix: combination of numbers & chars, num: just numbers </param>
        /// <returns></returns>
        public IActionResult Index(int l = 5, string bc = "Transparent", string fc = "random", string t = "mix")
        {
            _backgroundColor = bc;
            _forecolor = fc;
            _type = t;
            _length = l;
            Random autoRand = new Random();
            string randomStr = "";
            char[] myArray = new char[l];
            int x;

            for (x = 0; x < l; x++)
            {
                myArray[x] = RandomChar(autoRand);
                randomStr += myArray[x].ToString();
            }

            HttpContext.Session.SetString("captcha", randomStr);

            return File(CreateCAPTCHAImage(randomStr), "image/png");
        }

        #region Private Methods
        /// <summary>
        /// Generate random char sequences
        /// </summary>
        /// <param name="autoRand">Random generator class</param>
        /// <returns></returns>
        private char RandomChar(Random autoRand)
        {
            char c = '0';
            int start, end;
            if (_type == "mix")
            {
                start = 1;
                end = 4;
            } else //if (_type == "num")
            {
                start = 1;
                end = 2;
            }
            switch (autoRand.Next(start, end))
            {
                case 1:
                    c = System.Convert.ToChar(autoRand.Next(49, 58));

                    // excluding 0
                    break;
                case 2:
                    c = System.Convert.ToChar(autoRand.Next(65, 79));

                    // excluding o
                    break;
                case 3:
                    c = System.Convert.ToChar(autoRand.Next(80, 91));
                    break;
            }
            return c;
        }

        /// <summary>
        /// Generate foreground color of Captcha randomly
        /// </summary>
        /// <param name="autoRand">Random generator class</param>
        /// <returns></returns>
        private Color RandomforeColor()
        {
            if (_forecolor != "random")
                return Color.FromName(_forecolor);
            return RandomColor();
        }

        private Color RandomColor()
        {
            Random autoRand = new Random();
            return (autoRand.Next(1, 6)) switch
            {
                1 => Color.Crimson,
                2 => Color.Sienna,
                3 => Color.SeaGreen,
                4 => Color.Chocolate,
                5 => Color.Black,
                _ => Color.Brown,
            };
        }
        /// <summary>
        /// Transparent background, you can change it if you want something else.
        /// </summary>
        /// <returns>Color</returns>
        private Color RandomBackColor()
        {
            if (_backgroundColor != "random")
                return Color.FromName(_backgroundColor);
            return RandomColor();
        }

        /// <summary>
        /// Generate random char sequences as a bitmap
        /// </summary>
        /// <param name="text">Random chars</param>
        /// <param name="width">Width of the captcha</param>
        /// <param name="height">Height of the captcha</param>
        /// <param name="fontFamily">Font of the captcha</param>
        /// <returns>Bitmap byte array</returns>
        private byte[] CreateCAPTCHAImage(string text, int width = 120, int height = 40, string fontFamily = "Arial")
        {
            //HttpContext objHttpContext = HttpContext.Current;
            if (_length > 7)
                width += width / 7 * (_length - 7);


            Bitmap objBitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);


            Graphics objGraphics = Graphics.FromImage(objBitmap);
            Rectangle objRectangle = new Rectangle(0, 0, width, height);
            objGraphics.SmoothingMode = SmoothingMode.HighQuality;


            using (SolidBrush objSolidBrush = new SolidBrush(RandomBackColor())) { objGraphics.FillRectangle(objSolidBrush, objRectangle); }
            Font objFont = new Font(fontFamily, 25, FontStyle.Regular);
            SizeF objSizeF = new SizeF(0, 0); SizeF objSize = new SizeF(width, height);

            GraphicsPath objGraphicsPath = new GraphicsPath();
            StringFormat objStringFormat = new StringFormat();
            objStringFormat.Alignment = StringAlignment.Center;
            objStringFormat.LineAlignment = StringAlignment.Center;
            objGraphicsPath.AddString(text, objFont.FontFamily, (int)objFont.Style, objFont.Size, objRectangle, objStringFormat);


            SolidBrush objSolidBrushColor = new SolidBrush(RandomforeColor());
            objGraphics.FillPath(objSolidBrushColor, objGraphicsPath);
            #region Distortion

            double distort = new Random().Next(3, 6) * (new Random().Next(3) == 1 ? 1 : -1);
            using (Bitmap copy = (Bitmap)objBitmap.Clone())
            {
                for (int waveY = 0; waveY < height; waveY++)
                {
                    for (int waveX = 0; waveX < width; waveX++)
                    {
                        int newX = (int)(waveX + (distort * Math.Sin(Math.PI * waveY / 56.0)));
                        int newY = (int)(waveY + (distort * Math.Cos(Math.PI * waveX / 44.0)));
                        if (newX < 0 || newX >= width)
                            newX = 0; if (newY < 0 || newY >= height) newY = 0;
                        objBitmap.SetPixel(waveX, waveY, copy.GetPixel(newX, newY));
                    }
                }
            }
            #endregion Distortion
            //objHttpContext.Response.ContentType = "image/png";
            //objBitmap.Save(this.Response.OutputStream, ImageFormat.Png);
            Byte[] data;

            using (var memoryStream = new MemoryStream())
            {
                objBitmap.Save(memoryStream, ImageFormat.Png);
                data = memoryStream.ToArray();
            }
            return data;
        }

        #endregion    
    }
}
