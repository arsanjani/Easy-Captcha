#nullable enable

using EasyCaptcha.Enum;
using EasyCaptcha.Service;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;
using System.Collections.Generic;

namespace XUnitTest;

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
        var captchaService = new CaptchaService();
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
        var captchaService = new CaptchaService();
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
        var captchaService = new CaptchaService();
        var rnd = captchaService.GenerateRandomString(length, type);
        Regex rg = type == CharType.MIX 
            ? new Regex(@"^[a-zA-Z1-9]+$") 
            : new Regex(@"^[1-9]+$");
        Assert.Equal(expected, rg.IsMatch(rnd));
    }

    [Theory]
    [InlineData(0, CharType.MIX, " ", null)]
    [InlineData(0, CharType.NUM, " ", null)]
    [InlineData(-1, CharType.MIX, null, null)]
    [InlineData(-1, CharType.NUM, null, null)]
    public void ShouldReturnsEmptyByteArrayForEmptyString(int length, CharType type, string? backColor, string? foreColor)
    {
        var captchaService = new CaptchaService();
        var rnd = captchaService.GenerateRandomString(length, type);
        var image = captchaService.CreateCAPTCHAImage(rnd, backColor, foreColor);
        
        // With ImageSharp implementation, empty string returns empty array instead of null
        Assert.NotNull(image);
        Assert.Empty(image);
    }

    [Theory]
    [InlineData(1, CharType.MIX, " ", null)]
    [InlineData(1, CharType.NUM, "", null)]
    [InlineData(2, CharType.MIX, null, " ")]
    [InlineData(2, CharType.NUM, null, null)]
    [InlineData(20, CharType.MIX, "red", "random")]
    [InlineData(20, CharType.NUM, "$%$%^^&sasas", "saassadddff#$%^")]
    public void ShouldReturnsByteArrayOfRandomString(int length, CharType type, string? backColor, string? foreColor)
    {
        var captchaService = new CaptchaService();
        var rnd = captchaService.GenerateRandomString(length, type);
        var image = captchaService.CreateCAPTCHAImage(rnd, backColor, foreColor);
        Assert.NotNull(image);
        Assert.True(image.Length > 0);
    }

    [Theory]
    [InlineData("HELLO", "white", "black", 150, 50)]
    [InlineData("123", "transparent", "red", 100, 40)]
    [InlineData("A1B2C", "random", "random", 200, 60)]
    public void ShouldCreateValidPngImage(string text, string backColor, string foreColor, int width, int height)
    {
        var captchaService = new CaptchaService();
        var image = captchaService.CreateCAPTCHAImage(text, backColor, foreColor, width, height);
        
        Assert.NotNull(image);
        Assert.True(image.Length > 0);
        
        // Verify it's a valid PNG by checking the PNG signature
        Assert.True(image.Length >= 8);
        Assert.Equal(0x89, image[0]); // PNG signature byte 1
        Assert.Equal(0x50, image[1]); // PNG signature byte 2 (P)
        Assert.Equal(0x4E, image[2]); // PNG signature byte 3 (N)
        Assert.Equal(0x47, image[3]); // PNG signature byte 4 (G)
    }

    [Fact]
    public void ShouldGenerateDifferentImagesForSameText()
    {
        var captchaService = new CaptchaService();
        const string text = "TEST";
        
        var image1 = captchaService.CreateCAPTCHAImage(text, "random", "random");
        var image2 = captchaService.CreateCAPTCHAImage(text, "random", "random");
        
        Assert.NotNull(image1);
        Assert.NotNull(image2);
        Assert.True(image1.Length > 0);
        Assert.True(image2.Length > 0);
        
        // Images should be different due to random distortion and colors
        Assert.NotEqual(image1, image2);
    }

    [Theory]
    [InlineData("Arial")]
    [InlineData("Times New Roman")]
    [InlineData("Courier New")]
    [InlineData("NonExistentFont")] // Should fallback to default font
    public void ShouldHandleDifferentFontFamilies(string fontFamily)
    {
        var captchaService = new CaptchaService();
        var image = captchaService.CreateCAPTCHAImage("TEST", "white", "black", 150, 50, fontFamily);
        
        Assert.NotNull(image);
        Assert.True(image.Length > 0);
    }

    [Theory]
    [InlineData(5, CharType.MIX)]
    [InlineData(4, CharType.NUM)]
    [InlineData(6, CharType.MIX)]
    public void ShouldGenerateUniqueStrings(int length, CharType type)
    {
        var captchaService = new CaptchaService();
        var results = new HashSet<string>();
        
        // Generate multiple strings and ensure they're unique
        for (int i = 0; i < 100; i++)
        {
            var result = captchaService.GenerateRandomString(length, type);
            results.Add(result);
        }
        
        // We should have generated multiple unique strings
        Assert.True(results.Count > 1, "Should generate unique strings");
    }

    [Theory]
    [InlineData(50, 30)]
    [InlineData(200, 80)]
    [InlineData(300, 120)]
    public void ShouldHandleDifferentImageSizes(int width, int height)
    {
        var captchaService = new CaptchaService();
        var image = captchaService.CreateCAPTCHAImage("TEST", "white", "black", width, height);
        
        Assert.NotNull(image);
        Assert.True(image.Length > 0);
        
        // Verify it's a valid PNG
        Assert.True(image.Length >= 8);
        Assert.Equal(0x89, image[0]); // PNG signature
    }

    [Theory]
    [InlineData("")]
    public void ShouldHandleEmptyText(string text)
    {
        var captchaService = new CaptchaService();
        var image = captchaService.CreateCAPTCHAImage(text, "white", "black");
        
        Assert.NotNull(image);
        // For empty text, should return empty byte array
        Assert.Empty(image);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ShouldHandleWhitespaceText(string text)
    {
        var captchaService = new CaptchaService();
        var image = captchaService.CreateCAPTCHAImage(text, "white", "black");
        
        Assert.NotNull(image);
        // For whitespace text, should generate valid PNG image
        Assert.True(image.Length > 0);
        
        // Verify it's a valid PNG by checking the PNG signature
        Assert.True(image.Length >= 8);
        Assert.Equal(0x89, image[0]); // PNG signature byte 1
        Assert.Equal(0x50, image[1]); // PNG signature byte 2 (P)
        Assert.Equal(0x4E, image[2]); // PNG signature byte 3 (N)
        Assert.Equal(0x47, image[3]); // PNG signature byte 4 (G)
    }

    [Fact]
    public void ShouldHandleLongText()
    {
        var captchaService = new CaptchaService();
        var longText = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        var image = captchaService.CreateCAPTCHAImage(longText, "white", "black", 400, 60);
        
        Assert.NotNull(image);
        Assert.True(image.Length > 0);
        
        // Verify it's a valid PNG
        Assert.True(image.Length >= 8);
        Assert.Equal(0x89, image[0]); // PNG signature
    }
}
