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
    private readonly IConfiguration _configuration;

    [BindProperty]
    public RegisterModel Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public FirebaseConfig FirebaseConfig { get; set; } = new();

    public RegisterPageModel(IFirebaseAuthService firebaseAuthService, ILogger<RegisterPageModel> logger, IConfiguration configuration)
    {
        _firebaseAuthService = firebaseAuthService;
        _logger = logger;
        _configuration = configuration;
    }

    public void OnGet()
    {
        FirebaseConfig = _configuration.GetSection("Firebase").Get<FirebaseConfig>() ?? new FirebaseConfig();
    }

    public async Task<IActionResult> OnPostFirebaseTokenAsync([FromBody] FirebaseTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return new BadRequestObjectResult("Invalid token");
            }

            // Verify the Firebase ID token
            var decodedToken = await _firebaseAuthService.VerifyIdTokenAsync(request.IdToken);
            
            // Get user information
            var userRecord = await _firebaseAuthService.GetUserAsync(decodedToken.Uid);

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                new Claim(ClaimTypes.Email, userRecord.Email ?? decodedToken.Claims.GetValueOrDefault("email")?.ToString() ?? ""),
                new Claim(ClaimTypes.Name, userRecord.DisplayName ?? decodedToken.Claims.GetValueOrDefault("name")?.ToString() ?? "User"),
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

            _logger.LogInformation("User registered successfully with Firebase: {Email}", userRecord.Email);

            return new OkResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Firebase token during registration");
            return new BadRequestObjectResult(ex.Message);
        }
    }
}
