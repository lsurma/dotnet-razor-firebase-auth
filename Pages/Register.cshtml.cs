using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FirebaseAuthApp.Models;
using FirebaseAuthApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace FirebaseAuthApp.Pages;

public class RegisterPageModel : PageModel
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly ILogger<RegisterPageModel> _logger;

    [BindProperty]
    public RegisterModel Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public RegisterPageModel(IFirebaseAuthService firebaseAuthService, ILogger<RegisterPageModel> logger)
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
            var credential = await _firebaseAuthService.RegisterWithEmailPasswordAsync(
                Input.Email,
                Input.Password,
                Input.DisplayName ?? "User");

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, credential.User.Uid),
                new Claim(ClaimTypes.Email, credential.User.Info.Email ?? Input.Email),
                new Claim(ClaimTypes.Name, credential.User.Info.DisplayName ?? Input.DisplayName ?? "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User registered successfully: {Email}", Input.Email);

            return LocalRedirect(returnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            ErrorMessage = ex.Message;
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
