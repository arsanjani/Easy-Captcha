using EasyCaptcha.Enum;
using EasyCaptcha.Service;
using System;
using System.Text.RegularExpressions;
using Xunit;

namespace XUnitTest
{
    public class TestCase
    {
        [Theory]
        [InlineData(5, CharType.MIX, true)]
        [InlineData(7, CharType.MIX, true)]
        [InlineData(1, CharType.MIX, true)]
        [InlineData(10, CharType.NUM, true)]
        [InlineData(2, CharType.NUM, true)]
        [InlineData(15, CharType.NUM, true)]
        public void ShouldReturnsIndividualString(int length, CharType type, bool expected)
        {
            CaptchaService captchaService = new CaptchaService();
            var first = captchaService.GenerateRandomString(length, type);
            var second = captchaService.GenerateRandomString(length, type);
            var actual = first.Length == second.Length;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(6, CharType.MIX, 6)]
        [InlineData(6, CharType.NUM, 6)]
        [InlineData(0, CharType.MIX, 0)]
        [InlineData(0, CharType.NUM, 0)]
        [InlineData(15, CharType.MIX, 15)]
        [InlineData(15, CharType.NUM, 15)]
        [InlineData(-1, CharType.MIX, 0)]
        [InlineData(-1, CharType.NUM, 0)]
        public void ShouldReturnsSameLengthAsExpected(int length, CharType type, int expected)
        {
            CaptchaService captchaService = new CaptchaService();
            var rnd = captchaService.GenerateRandomString(length, type);
            Assert.Equal(expected, rnd.Length);
        }

        [Theory]
        [InlineData(-1, CharType.MIX, false)]
        [InlineData(-1, CharType.NUM, false)]
        [InlineData(0, CharType.MIX, false)]
        [InlineData(0, CharType.NUM, false)]
        [InlineData(5, CharType.MIX, true)]
        [InlineData(5, CharType.NUM, true)]
        [InlineData(20, CharType.MIX, true)]
        [InlineData(20, CharType.NUM, true)]
        [InlineData(1, CharType.MIX, true)]
        [InlineData(1, CharType.NUM, true)]
        [InlineData(3, CharType.MIX, true)]
        [InlineData(3, CharType.NUM, true)]
        [InlineData(4, CharType.MIX, true)]
        [InlineData(4, CharType.NUM, true)]
        public void ShouldReturnsBasedOnType(int length, CharType type, bool expected)
        {
            CaptchaService captchaService = new CaptchaService();
            var rnd = captchaService.GenerateRandomString(length, type);
            Regex rg;
            if (type == CharType.MIX)
                rg = new Regex(@"^[a-zA-Z1-9]+$");
            else
                rg = new Regex(@"^[1-9]+$");
            Assert.Equal(expected, rg.IsMatch(rnd));
        }

        [Theory]
        [InlineData(0, CharType.MIX, " ", null)]
        [InlineData(0, CharType.NUM, " ", null)]
        [InlineData(-1, CharType.MIX, null, null)]
        [InlineData(-1, CharType.NUM, null, null)]
        public void ShouldReturnsEmptyByteArray(int length, CharType type, string backColor, string foreColor)
        {
            CaptchaService captchaService = new CaptchaService();
            var rnd = captchaService.GenerateRandomString(length, type);
            var image = captchaService.CreateCAPTCHAImage(rnd, backColor, foreColor);
            Assert.Null(image);
        }

        [Theory]
        [InlineData(1, CharType.MIX, " ", null)]
        [InlineData(1, CharType.NUM, "", null)]
        [InlineData(2, CharType.MIX, null, " ")]
        [InlineData(2, CharType.NUM, null, null)]
        [InlineData(20, CharType.MIX, "red", "random")]
        [InlineData(20, CharType.NUM, "$%$%^^&sasas", "saassadddff#$%^")]
        public void ShouldReturnsByteArrayOfRandomString(int length, CharType type, string backColor, string foreColor)
        {
            CaptchaService captchaService = new CaptchaService();
            var rnd = captchaService.GenerateRandomString(length, type);
            var image = captchaService.CreateCAPTCHAImage(rnd, backColor, foreColor);
            Assert.NotNull(image);
            Assert.True(image.Length > 0);
        }


    }
}
