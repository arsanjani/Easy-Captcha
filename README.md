# Easy Captcha
An efficient and secured alternative of Google reCAPTCHA for ASP.NET Core and ASP.NET MVC which has been written with C#. If you are being tired of struggling with Google reCAPTCHA and also worried about performance and banning issue with it, just take a minute to go through it for your consideration. I am certain that you won't be regret.


![c1](https://user-images.githubusercontent.com/8726637/149615831-226a2616-0a2c-4932-aaa9-61bdaed5cbd9.PNG)
![c2](https://user-images.githubusercontent.com/8726637/149615840-5db002d1-4380-491a-b349-c3611e3a286c.PNG)
![c3](https://user-images.githubusercontent.com/8726637/149615844-743957c5-4971-4176-8fac-d4bb9d622ab4.PNG)
![c4](https://user-images.githubusercontent.com/8726637/149615876-27b9ca2c-efd2-452f-baf9-471ead380f51.PNG)
![c5](https://user-images.githubusercontent.com/8726637/149615879-028c072e-6952-49ea-b108-745c4549d62b.PNG)
![c6](https://user-images.githubusercontent.com/8726637/149615881-95277c22-4196-4915-8d9f-9ff06e167e69.PNG)
![c7](https://user-images.githubusercontent.com/8726637/149615893-fd201df5-fa57-448e-ad99-c1e5a2d70bce.PNG)
![c8](https://user-images.githubusercontent.com/8726637/149615897-7f6f9742-fabc-441f-b9c8-f5ba14b83a1e.png)
![c9](https://user-images.githubusercontent.com/8726637/149615898-be83de38-9292-422d-89fc-112a8ec9f36c.png)
![c10](https://user-images.githubusercontent.com/8726637/149615900-5e02d9c0-aebd-4fe5-8455-f5977ae4baab.png)

# How to use
* Install it from `Nuget`:
  ```
  Install-Package EasyCaptchaCore -Version 1.0.0
  ```
* Add `ICaptchaService` to your `Statrup` like this:
  ```C#
  services.AddTransient<ICaptchaService, CaptchaService>();
  ```
* Enable `Session` in `Configure` method like this:
  ```C#
  app.UseSession();
  ```
* Enable session `IdleTimeout` in your `ConfigureServices` If you are using `.NET Core` or `MVC`. You can change the session timeout if you want. It uses to store the `Captcha` in the user session securely.

  ```C#
  services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
  ```            
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
* You can change the length of the captcha by the code below. (Default value is 5):
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
