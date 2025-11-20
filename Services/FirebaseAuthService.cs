using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using FirebaseAuthApp.Models;

namespace FirebaseAuthApp.Services;

public interface IFirebaseAuthService
{
    Task<FirebaseToken> VerifyIdTokenAsync(string idToken);
    Task<UserRecord> GetUserAsync(string uid);
}

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuth _auth;
    private readonly ILogger<FirebaseAuthService> _logger;

    public FirebaseAuthService(IConfiguration configuration, ILogger<FirebaseAuthService> logger)
    {
        _logger = logger;
        var firebaseConfig = configuration.GetSection("Firebase").Get<FirebaseConfig>();
        
        if (firebaseConfig == null || string.IsNullOrEmpty(firebaseConfig.ProjectId))
        {
            throw new InvalidOperationException("Firebase configuration is not properly set in appsettings.json");
        }

        // Initialize Firebase Admin SDK if not already initialized
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromAccessToken(null),
                ProjectId = firebaseConfig.ProjectId
            });
        }

        _auth = FirebaseAuth.DefaultInstance;
    }

    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            var decodedToken = await _auth.VerifyIdTokenAsync(idToken);
            return decodedToken;
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Error verifying Firebase ID token");
            throw new InvalidOperationException("Invalid ID token", ex);
        }
    }

    public async Task<UserRecord> GetUserAsync(string uid)
    {
        try
        {
            return await _auth.GetUserAsync(uid);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Error getting user from Firebase");
            throw new InvalidOperationException("Unable to get user", ex);
        }
    }
}
