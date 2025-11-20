using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FirebaseAuthApp.Models;
using FirebaseAuthApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace FirebaseAuthApp.Pages;

public class LoginPageModel : PageModel
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly ILogger<LoginPageModel> _logger;

    [BindProperty]
    public LoginModel Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public LoginPageModel(IFirebaseAuthService firebaseAuthService, ILogger<LoginPageModel> logger)
    {
        _firebaseAuthService = firebaseAuthService;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var credential = await _firebaseAuthService.SignInWithEmailPasswordAsync(
                Input.Email,
                Input.Password);

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, credential.User.Uid),
                new Claim(ClaimTypes.Email, credential.User.Info.Email ?? Input.Email),
                new Claim(ClaimTypes.Name, credential.User.Info.DisplayName ?? "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                ExpiresUtc = Input.RememberMe 
                    ? DateTimeOffset.UtcNow.AddDays(30) 
                    : DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User logged in successfully: {Email}", Input.Email);

            return LocalRedirect(returnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            ErrorMessage = "Invalid email or password.";
            ModelState.AddModelError(string.Empty, ErrorMessage);
            return Page();
        }
    }

    public IActionResult OnPostGoogleLogin()
    {
        var properties = new AuthenticationProperties 
        { 
            RedirectUri = Url.Page("/Login", pageHandler: "GoogleResponse")
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> OnGetGoogleResponseAsync()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        if (!result.Succeeded)
        {
            ErrorMessage = "Google authentication failed.";
            return RedirectToPage();
        }

        return RedirectToPage("/Index");
    }
}
