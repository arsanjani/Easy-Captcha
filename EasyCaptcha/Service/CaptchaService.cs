using EasyCaptcha.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCaptcha.Service
{
    public class CaptchaService: ICaptchaService
    {
        public string GenerateRandomString(int length, CharType type)
        {
            if (length < 1)
                return string.Empty;
            Random autoRand = new Random();
            string randomStr = "";
            char[] myArray = new char[length];
            int x;

            for (x = 0; x < length; x++)
            {
                myArray[x] = RandomChar(autoRand, type);
                randomStr += myArray[x].ToString();
            }
            return randomStr;
        }

        /// <summary>
        /// Generate random char sequences
        /// </summary>
        /// <param name="autoRand">Random generator class</param>
        /// <returns></returns>
        private char RandomChar(Random autoRand, CharType type)
        {
            char c = '0';
            int start, end;
            if (type == CharType.MIX)
            {
                start = 1;
                end = 4;
            }
            else 
            {
                start = 1;
                end = 2;
            }
            switch (autoRand.Next(start, end))
            {
                case 1:
                    c = Convert.ToChar(autoRand.Next(49, 58));

                    // excluding 0
                    break;
                case 2:
                    c = Convert.ToChar(autoRand.Next(65, 79));

                    // excluding o
                    break;
                case 3:
                    c = Convert.ToChar(autoRand.Next(80, 91));
                    break;
            }
            return c;
        }


        /// <summary>
        /// Generate random color
        /// </summary>
        /// <returns></returns>
        private Color RandomColor()
        {
            Random rnd = new Random();
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            //return (autoRand.Next(1, 6)) switch
            //{
            //    1 => Color.Crimson,
            //    2 => Color.Sienna,
            //    3 => Color.SeaGreen,
            //    4 => Color.Chocolate,
            //    5 => Color.Black,
            //    _ => Color.Brown,
            //};
        }
        /// <summary>
        /// Transparent background, you can change it if you want something else.
        /// </summary>
        /// <returns>Color</returns>
        private Color GetColor(string color)
        {
            if (color != "random" && !string.IsNullOrWhiteSpace(color))
                return Color.FromName(color);
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
        public byte[] CreateCAPTCHAImage(string text, string backgroundColor, string foreColor, int width = 120, int height = 40, string fontFamily = "Arial" )
        {
            if (string.IsNullOrEmpty(text))
                return null;
            if (text.Length > 6)
                width += width / 4 * (text.Length - 6);


            Bitmap objBitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);


            Graphics objGraphics = Graphics.FromImage(objBitmap);
            Rectangle objRectangle = new Rectangle(0, 0, width, height);
            objGraphics.SmoothingMode = SmoothingMode.HighQuality;


            using (SolidBrush objSolidBrush = new SolidBrush(GetColor(backgroundColor))) { objGraphics.FillRectangle(objSolidBrush, objRectangle); }
            Font objFont = new Font(fontFamily, 25, FontStyle.Regular);
            SizeF objSizeF = new SizeF(0, 0); SizeF objSize = new SizeF(width, height);

            GraphicsPath objGraphicsPath = new GraphicsPath();
            StringFormat objStringFormat = new StringFormat();
            objStringFormat.Alignment = StringAlignment.Center;
            objStringFormat.LineAlignment = StringAlignment.Center;
            objGraphicsPath.AddString(text, objFont.FontFamily, (int)objFont.Style, objFont.Size, objRectangle, objStringFormat);


            SolidBrush objSolidBrushColor = new SolidBrush(GetColor(foreColor));
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
    }
}
