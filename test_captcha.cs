using EasyCaptcha.Service;
using EasyCaptcha.Enum;
using System;
using System.IO;

// Simple test to verify CAPTCHA generation
var captchaService = new CaptchaService();

// Test string generation
var randomString = captchaService.GenerateRandomString(5, CharType.MIX);
Console.WriteLine($"Generated random string: {randomString}");

// Test image generation
var imageBytes = captchaService.CreateCAPTCHAImage(randomString, "white", "black", 150, 50, "Arial");

if (imageBytes.Length > 0)
{
    Console.WriteLine($"Successfully generated CAPTCHA image: {imageBytes.Length} bytes");
    
    // Save to file for verification
    File.WriteAllBytes("test_captcha.png", imageBytes);
    Console.WriteLine("Test CAPTCHA saved as 'test_captcha.png'");
}
else
{
    Console.WriteLine("Failed to generate CAPTCHA image");
}

Console.WriteLine("All tests completed successfully!"); 