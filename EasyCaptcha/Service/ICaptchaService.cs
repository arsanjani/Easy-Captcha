using EasyCaptcha.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCaptcha.Service
{
    public interface ICaptchaService
    {
        string GenerateRandomString(int length, CharType type);
        byte[] CreateCAPTCHAImage(string text, string backgroundColor, string foreColor, int width = 120, int height = 40, string fontFamily = "Arial");
    }
}
