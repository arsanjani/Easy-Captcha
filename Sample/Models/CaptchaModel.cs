using System.ComponentModel.DataAnnotations;

namespace Sample.Models;

public class CaptchaModel
{
    [Required(ErrorMessage = "Please enter the captcha text")]
    [Display(Name = "Captcha")]
    public string Captcha { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
}
