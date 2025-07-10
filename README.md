# Easy Captcha

An efficient and secured cross-platform alternative to Google reCAPTCHA for ASP.NET Core, written in C#. If you're tired of struggling with Google reCAPTCHA and worried about performance and banning issues, take a minute to consider this solution. Built with modern .NET 9 and cross-platform ImageSharp technology.

## 🚀 Features

- **✅ Cross-Platform**: Works on Windows, Linux, and macOS using SixLabors.ImageSharp
- **✅ .NET 9 Compatible**: Built with the latest .NET 9 and modern C# patterns
- **✅ Zero Dependencies**: No external services or APIs required
- **✅ Lightweight**: Minimal footprint with high performance
- **✅ Customizable**: Colors, fonts, sizes, and character types
- **✅ Secure**: Session-based verification with configurable timeout
- **✅ Comprehensive Testing**: 57+ test cases ensuring reliability

## 📸 Examples

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

## 🛠️ Installation

Install the package from NuGet:

```bash
dotnet add package EasyCaptchaCore --version 1.0.1
```

Or via Package Manager Console:

```powershell
Install-Package EasyCaptchaCore -Version 1.0.1
```

## 🔧 Setup (.NET 9)

### 1. Configure Services in Program.cs

```csharp
using EasyCaptcha.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

// Add EasyCaptcha service
builder.Services.AddTransient<ICaptchaService, CaptchaService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Enable session before authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### 2. Add Captcha to Your View

Basic implementation:

```html
<img src="~/Captcha" alt="Captcha" />
```

With click-to-refresh functionality:

```html
<img id="captchaImage" src="~/Captcha" alt="Captcha" 
     style="cursor: pointer;" onclick="refreshCaptcha()" />

<script>
function refreshCaptcha() {
    const img = document.getElementById('captchaImage');
    const timestamp = new Date().getTime();
    img.src = `/Captcha?_=${timestamp}`;
}
</script>
```

### 3. Verify Captcha in Controller

```csharp
[HttpPost]
public IActionResult VerifyCaptcha([FromBody] CaptchaRequest request)
{
    var sessionCaptcha = HttpContext.Session.GetString("captcha");
    
    if (string.IsNullOrEmpty(sessionCaptcha) || string.IsNullOrEmpty(request.Captcha))
    {
        return Json(new { success = false, message = "Invalid captcha" });
    }

    var isValid = string.Equals(sessionCaptcha, request.Captcha, 
                               StringComparison.OrdinalIgnoreCase);
    
    // Clear session after verification
    HttpContext.Session.Remove("captcha");
    
    return Json(new { success = isValid });
}

public class CaptchaRequest 
{
    public string Captcha { get; set; } = string.Empty;
}
```

## 🎨 Customization Options

### Length
Change the number of characters (default: 5):
```html
<img src="~/Captcha?l=6" alt="Captcha" />
```

### Background Color
Set background color by name or use "random" (default: "transparent"):
```html
<img src="~/Captcha?bc=white" alt="Captcha" />
<img src="~/Captcha?bc=random" alt="Captcha" />
```

### Foreground Color
Set text color by name or use "random" (default: "random"):
```html
<img src="~/Captcha?fc=black" alt="Captcha" />
<img src="~/Captcha?fc=random" alt="Captcha" />
```

### Character Type
Choose between mixed alphanumeric or numbers only (default: "MIX"):
```html
<img src="~/Captcha?t=NUM" alt="Captcha" />  <!-- Numbers only -->
<img src="~/Captcha?t=MIX" alt="Captcha" />  <!-- Mixed characters -->
```

### Image Size
Customize width and height (default: 120x40):
```html
<img src="~/Captcha?width=200&height=60" alt="Captcha" />
```

### Font Family
Specify font family (default: "Arial"):
```html
<img src="~/Captcha?fontFamily=Times%20New%20Roman" alt="Captcha" />
```

### Combined Example
```html
<img src="~/Captcha?l=6&bc=white&fc=blue&t=MIX&width=180&height=50" alt="Captcha" />
```

## 🎯 Supported Colors

- **Named Colors**: white, black, red, green, blue, yellow, cyan, magenta, gray, transparent
- **Random**: Use "random" for random color generation
- **Transparent**: Use "transparent" for transparent background

## 🧪 Testing

The project includes comprehensive testing with 57+ test cases:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 Project Structure

```
EasyCaptcha/
├── EasyCaptcha/              # Main library
│   ├── Controllers/          # Captcha controller
│   ├── Service/              # Captcha service implementation
│   └── Enum/                 # Character type definitions
├── Sample/                   # Demo web application
│   ├── Controllers/          # Sample controllers
│   ├── Views/                # Sample views
│   └── Models/               # Sample models
└── XUnitTest/                # Comprehensive test suite
```

## 🚀 Demo Application

Run the included sample application:

```bash
dotnet run --project Sample/Sample.csproj
```

Features:
- Interactive CAPTCHA verification
- Real-time refresh functionality
- Multiple configuration examples
- AJAX-based form submission
- Modern responsive UI

## 📋 Requirements

- **.NET 9.0** or later
- **ASP.NET Core 9.0** or later
- **Session support** enabled

## 🔧 Dependencies

- **SixLabors.ImageSharp** (3.1.6) - Cross-platform image processing
- **SixLabors.ImageSharp.Drawing** (2.1.5) - Drawing capabilities
- **SixLabors.Fonts** (2.1.0) - Font support

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📞 Support

If you encounter any issues or have questions:
- Create an issue on [GitHub](https://github.com/arsanjani/Easy-Captcha/issues)
- Check the [Sample project](Sample/) for implementation examples
- Review the [test cases](XUnitTest/) for usage patterns

## 🎉 Acknowledgments

- Built with modern .NET 9 and C# patterns
- Cross-platform compatibility thanks to SixLabors.ImageSharp
- Comprehensive testing ensuring reliability
- Community feedback and contributions

---

**Easy Captcha** - Making CAPTCHA simple, secure, and cross-platform! 🛡️✨
