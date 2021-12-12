# Easy Captcha
An efficient and secured alternative of Google reCAPTCHA for ASP.NET Core and ASP.NET MVC which has been written with C#. If you are being tired of struggling with Google reCAPTCHA and also worried about performance and banning issue with it, just take a minute to go through it for your consideration. I am certain that you won't be regret.

![Easy Captcha](/EasyCaptcha/wwwroot/img/easyCaptcha.PNG)

# How to use
* This project uses `Bitmap` to create a temporary image of random characters. In order to do that, you need to add `System.Drawing.Common` to your project. Use the command below to install it from `Nuget`:
  ```
  Install-Package System.Drawing.Common
  ```
* Enable session in your `ConfigureServices` If you are using `.NET Core` or `MVC`. You can change the session timeout if you want. It uses to store the `Captcha` in the user session securely.

  ```C#
  services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
  ```            
* Copy `CaptchaController.cs` to your project.
* Use the code below in your `View Layout`:

  ```HTML
  <img src="~/Captcha" alt="Captcha" />
  ```
* In order to randomly change the `Captcha` text by clicking on it, you can use the code below:

  ```HTML
  <img src="~/Captcha" alt="Captcha" style="cursor: pointer;" onclick="refreshImage(this);" />
  ```
  ```Javascript
  <script type="text/javascript">
            refreshImage = function (img) {
                var source = img.src;
                img.src = '';

                source = source.split('?')[0];
                source = source + '?' + new Date().getTime();

                img.src = source;
            }
  </script>
  ```
* Use the code below to compare the real `Captcha` with the user input:

  ```C#
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
  ```
* You can change the length of the captcha by the code below:
  ```HTML
  <img src="~/Captcha?l=6" alt="Captcha" />
  ```
* You can change the background color of the captcha by the code below either by writting the color name or the keyword `random`. (Default is `transparent`):
  ```HTML
  <img src="~/Captcha?bc=black" alt="Captcha" />
  ```  
  ```HTML
  <img src="~/Captcha?bc=random" alt="Captcha" />
  ```  
* You can change the forecolor of the captcha by the code below either by writting the color name or the keyword `random`. (Default is `random`):
  ```HTML
  <img src="~/Captcha?fc=green" alt="Captcha" />
  ```
* You can change type of the characters. `num` for numeric characters or `mix` for mixed characters. (Default is `mix`):
  ```HTML
  <img src="~/Captcha?t=num" alt="Captcha" />
  ```
* You can change the `Captcha` length, forecolor, background color, add more noisy line to make it more complicated, and so on so forth very easily in `CaptchaController.cs`.
