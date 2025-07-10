using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System;
using System.Diagnostics;

namespace Sample.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult VerifyCaptcha([FromBody] CaptchaVerificationRequest request)
    {
        try
        {
            var sessionCaptcha = HttpContext.Session.GetString("captcha");
            
            if (string.IsNullOrEmpty(sessionCaptcha) || string.IsNullOrEmpty(request.Captcha))
            {
                return Json(new { success = false, message = "CAPTCHA not found or invalid" });
            }

            var isValid = string.Equals(sessionCaptcha, request.Captcha, StringComparison.OrdinalIgnoreCase);
            
            // Clear the session captcha after verification
            HttpContext.Session.Remove("captcha");
            
            return Json(new { success = isValid, message = isValid ? "CAPTCHA verified successfully" : "CAPTCHA verification failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during CAPTCHA verification");
            return Json(new { success = false, message = "An error occurred during verification" });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class CaptchaVerificationRequest
{
    public string Captcha { get; set; } = string.Empty;
}
