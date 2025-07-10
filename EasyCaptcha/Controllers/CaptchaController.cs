using EasyCaptcha.Enum;
using EasyCaptcha.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyCaptcha.Controllers;

[AllowAnonymous]
public class CaptchaController(ICaptchaService captchaService) : Controller
{
    private readonly ICaptchaService _captchaService = captchaService;

    /// <summary>
    /// Returns a random Captcha as a bitmap
    /// You can change Captcha length here if you want.
    /// </summary>
    /// <param name="l">Length of the captcha. Default is 5</param>
    /// <param name="bc">Background color of the captcha. Default is Transparent. You can use random value</param>
    /// <param name="fc">Forecolor of the texts. You can use random value</param>
    /// <param name="t">mix: combination of numbers & chars, num: just numbers</param>
    /// <returns>An image contains random captcha</returns>
    public IActionResult Index(int l = 5, string bc = "transparent", string fc = "random", CharType t = CharType.MIX)
    {
        string randomStr = _captchaService.GenerateRandomString(l, t);

        HttpContext.Session.SetString("captcha", randomStr);

        return File(_captchaService.CreateCAPTCHAImage(randomStr, bc, fc), "image/png");
    }
}
