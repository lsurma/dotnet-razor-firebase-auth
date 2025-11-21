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
    private readonly string _projectId;

    public FirebaseAuthService(IConfiguration configuration, ILogger<FirebaseAuthService> logger)
    {
        _logger = logger;
        var firebaseConfig = configuration.GetSection("Firebase").Get<FirebaseConfig>();
        
        if (firebaseConfig == null || string.IsNullOrEmpty(firebaseConfig.ProjectId))
        {
            throw new InvalidOperationException("Firebase configuration is not properly set in appsettings.json");
        }

        _projectId = firebaseConfig.ProjectId;

        // Initialize Firebase Admin SDK if not already initialized
        // Note: For ID token verification, we don't need service account credentials
        // Firebase Admin SDK can verify tokens using public keys
        if (FirebaseApp.DefaultInstance == null)
        {
            try
            {
                FirebaseApp.Create(new AppOptions()
                {
                    ProjectId = _projectId
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error initializing Firebase Admin SDK, attempting to use existing instance");
            }
        }

        _auth = FirebaseAuth.DefaultInstance;
    }

    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            // VerifyIdTokenAsync automatically checks:
            // 1. Token signature
            // 2. Token expiration (exp claim)
            // 3. Token issuer
            // 4. Token audience (project ID)
            // If checkRevoked parameter is true, it also checks if the token has been revoked
            var decodedToken = await _auth.VerifyIdTokenAsync(idToken, checkRevoked: true);
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
