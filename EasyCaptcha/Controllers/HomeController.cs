using EasyCaptcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EasyCaptcha.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new CaptchaModel());
        }
        
        [HttpPost]
        public ActionResult Index(CaptchaModel model)
        {
            var realCaptcha = HttpContext.Session.GetString("captcha").ToLower();
            if (realCaptcha != model.Captcha)
                model.Message = "Ops...Wrong captcha!";
            else
                model.Message = "Congrats! Captcha has been matched!";
            return View(model);
        }

        
    }
}
