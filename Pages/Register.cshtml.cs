using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FirebaseAuthApp.Models;
using FirebaseAuthApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace FirebaseAuthApp.Pages;

public class RegisterPageModel : PageModel
{
    private const string NewsletterSubscriptionClaimType = "newsletter_subscription";
    
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
                new Claim(NewsletterSubscriptionClaimType, Input.SubscribeToNewsletter.ToString().ToLower())
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

            _logger.LogInformation("User registered successfully: {Email}, Newsletter: {Newsletter}", 
                Input.Email, Input.SubscribeToNewsletter);

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

    public IActionResult OnPostGoogleRegister()
    {
        var properties = new AuthenticationProperties 
        { 
            RedirectUri = Url.Page("/Register", pageHandler: "GoogleResponse")
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> OnGetGoogleResponseAsync()
    {
        // Authenticate using the external authentication scheme
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        
        if (!result.Succeeded)
        {
            ErrorMessage = "Google authentication failed.";
            return RedirectToPage();
        }

        // Extract user information from the external authentication result
        var externalUser = result.Principal;
        if (externalUser == null)
        {
            ErrorMessage = "Unable to retrieve user information from Google.";
            return RedirectToPage();
        }

        // Validate required claims
        var nameIdentifier = externalUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = externalUser.FindFirst(ClaimTypes.Email)?.Value;
        var name = externalUser.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(nameIdentifier) || string.IsNullOrEmpty(email))
        {
            ErrorMessage = "Unable to retrieve required information from Google account.";
            return RedirectToPage();
        }

        // Create claims for the user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name ?? email),
            new Claim(NewsletterSubscriptionClaimType, "false") // Default newsletter to false for social registration
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        // Sign in the user with cookie authentication
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("User registered with Google successfully: {Email}", email);

        return RedirectToPage("/Index");
    }
}
