using EasyCaptcha.Enum;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Numerics;

namespace EasyCaptcha.Service;

public class CaptchaService : ICaptchaService
{
    private static readonly FontCollection FontCollection = new();
    private static readonly FontFamily DefaultFontFamily = SystemFonts.Get("Arial");

    public string GenerateRandomString(int length, CharType type)
    {
        if (length < 1)
            return string.Empty;
        
        var stringBuilder = new System.Text.StringBuilder(length);
        
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(GenerateRandomChar(type));
        }
        
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Generate random char sequences
    /// </summary>
    /// <param name="type">Type of characters to generate</param>
    /// <returns>Random character based on type</returns>
    private static char GenerateRandomChar(CharType type)
    {
        var range = type switch
        {
            CharType.MIX => Random.Shared.Next(1, 4),
            CharType.NUM => 1,
            _ => 1
        };

        return range switch
        {
            1 => Convert.ToChar(Random.Shared.Next(49, 58)), // Numbers 1-9 (excluding 0)
            2 => Convert.ToChar(Random.Shared.Next(65, 79)), // Letters A-N (excluding O)
            3 => Convert.ToChar(Random.Shared.Next(80, 91)), // Letters P-Z
            _ => '1'
        };
    }

    /// <summary>
    /// Generate random color
    /// </summary>
    /// <returns>Random color</returns>
    private static Color GenerateRandomColor()
    {
        return Color.FromRgb(
            (byte)Random.Shared.Next(256), 
            (byte)Random.Shared.Next(256), 
            (byte)Random.Shared.Next(256));
    }

    /// <summary>
    /// Get color by name or generate random color
    /// </summary>
    /// <param name="color">Color name or "random"</param>
    /// <returns>Color object</returns>
    private static Color GetColor(string color)
    {
        if (color != "random" && !string.IsNullOrWhiteSpace(color))
        {
            // Try to parse named colors
            return color.ToLowerInvariant() switch
            {
                "transparent" => Color.Transparent,
                "white" => Color.White,
                "black" => Color.Black,
                "red" => Color.Red,
                "green" => Color.Green,
                "blue" => Color.Blue,
                "yellow" => Color.Yellow,
                "cyan" => Color.Cyan,
                "magenta" => Color.Magenta,
                "gray" => Color.Gray,
                "grey" => Color.Gray,
                _ => GenerateRandomColor()
            };
        }
        
        return GenerateRandomColor();
    }

    /// <summary>
    /// Generate random char sequences as a bitmap
    /// </summary>
    /// <param name="text">Random chars</param>
    /// <param name="backgroundColor">Background color name or "random"</param>
    /// <param name="foreColor">Foreground color name or "random"</param>
    /// <param name="width">Width of the captcha</param>
    /// <param name="height">Height of the captcha</param>
    /// <param name="fontFamily">Font family name</param>
    /// <returns>Bitmap byte array</returns>
    public byte[] CreateCAPTCHAImage(string text, string backgroundColor, string foreColor, int width = 120, int height = 40, string fontFamily = "Arial")
    {
        if (string.IsNullOrEmpty(text))
            return [];

        if (text.Length > 6)
            width += width / 4 * (text.Length - 6);

        using var image = new Image<Rgba32>(width, height);
        
        var backgroundColor_ = GetColor(backgroundColor);
        var foregroundColor = GetColor(foreColor);
        
        // Get font
        var font = GetFont(fontFamily, 25);

        image.Mutate(ctx =>
        {
            // Fill background
            ctx.Fill(backgroundColor_);

            // Add distortion lines first (behind text)
            AddDistortionLines(ctx, width, height);

            // Draw text
            var textOptions = new RichTextOptions(font)
            {
                Origin = new Vector2(width / 2f, height / 2f),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            ctx.DrawText(textOptions, text, foregroundColor);

            // Apply wave distortion
            ApplyWaveDistortion(ctx, width, height);
        });

        // Convert to byte array
        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, new PngEncoder());
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Get font by family name and size
    /// </summary>
    /// <param name="fontFamily">Font family name</param>
    /// <param name="size">Font size</param>
    /// <returns>Font object</returns>
    private static Font GetFont(string fontFamily, float size)
    {
        try
        {
            var family = SystemFonts.Get(fontFamily);
            return family.CreateFont(size, FontStyle.Regular);
        }
        catch
        {
            // Fallback to default font if requested font is not available
            return DefaultFontFamily.CreateFont(size, FontStyle.Regular);
        }
    }

    /// <summary>
    /// Add random distortion lines to the image
    /// </summary>
    /// <param name="ctx">Image processing context</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    private static void AddDistortionLines(IImageProcessingContext ctx, int width, int height)
    {
        var pen = Pens.Solid(Color.Gray, 1);
        
        for (int i = 0; i < 10; i++)
        {
            var x0 = Random.Shared.Next(0, width);
            var y0 = Random.Shared.Next(0, height);
            var x1 = Random.Shared.Next(0, width);
            var y1 = Random.Shared.Next(0, height);
            
            ctx.DrawLine(pen, new Vector2(x0, y0), new Vector2(x1, y1));
        }
    }

    /// <summary>
    /// Apply wave distortion to the image
    /// </summary>
    /// <param name="ctx">Image processing context</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    private static void ApplyWaveDistortion(IImageProcessingContext ctx, int width, int height)
    {
        var distortionStrength = Random.Shared.Next(3, 6) * (Random.Shared.Next(2) == 0 ? -1 : 1);
        
        // Apply a wave transformation
        ctx.Transform(
            new AffineTransformBuilder()
                .PrependMatrix(Matrix3x2.CreateTranslation(-width / 2f, -height / 2f))
                .AppendMatrix(Matrix3x2.CreateTranslation(width / 2f, height / 2f)));

        // Apply subtle skew for additional distortion
        var skewX = (float)(Random.Shared.NextDouble() * 0.2 - 0.1); // -0.1 to 0.1
        var skewY = (float)(Random.Shared.NextDouble() * 0.2 - 0.1); // -0.1 to 0.1
        
        ctx.Skew(skewX, skewY);
    }
}
