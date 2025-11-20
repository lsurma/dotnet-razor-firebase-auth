using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAuthApp.Models;

namespace FirebaseAuthApp.Services;

public interface IFirebaseAuthService
{
    Task<UserCredential> RegisterWithEmailPasswordAsync(string email, string password, string displayName);
    Task<UserCredential> SignInWithEmailPasswordAsync(string email, string password);
    Task SignOutAsync();
}

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuthClient _authClient;

    public FirebaseAuthService(IConfiguration configuration)
    {
        var firebaseConfig = configuration.GetSection("Firebase").Get<FirebaseConfig>();
        
        if (firebaseConfig == null || string.IsNullOrEmpty(firebaseConfig.ApiKey))
        {
            throw new InvalidOperationException("Firebase configuration is not properly set in appsettings.json");
        }

        var config = new FirebaseAuthConfig
        {
            ApiKey = firebaseConfig.ApiKey,
            AuthDomain = firebaseConfig.AuthDomain,
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        _authClient = new FirebaseAuthClient(config);
    }

    public async Task<UserCredential> RegisterWithEmailPasswordAsync(string email, string password, string displayName)
    {
        try
        {
            var credential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password, displayName);
            return credential;
        }
        catch (FirebaseAuthException ex)
        {
            throw new InvalidOperationException($"Registration failed: {ex.Reason}", ex);
        }
    }

    public async Task<UserCredential> SignInWithEmailPasswordAsync(string email, string password)
    {
        try
        {
            var credential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            return credential;
        }
        catch (FirebaseAuthException ex)
        {
            throw new InvalidOperationException($"Sign in failed: {ex.Reason}", ex);
        }
    }

    public Task SignOutAsync()
    {
        // Firebase client-side sign out is handled on the client
        return Task.CompletedTask;
    }
}
