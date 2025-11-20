# .NET 8 Razor Pages Firebase Authentication

A complete .NET 8 Razor Pages application demonstrating Firebase authentication with custom login and register pages using OIDC and Google Firebase Authentication.

## Features

- ✅ Custom Login page with Firebase SDK
- ✅ Custom Register page with Firebase SDK
- ✅ Email/Password authentication
- ✅ Google OAuth authentication (OIDC)
- ✅ Custom CSS styling
- ✅ Session management with cookies
- ✅ User-friendly UI with gradient design
- ✅ Responsive layout

## Prerequisites

- .NET 8.0 SDK or later
- Firebase account
- Google Cloud Console account (for OAuth)

## Firebase Setup

1. **Create a Firebase Project**
   - Go to [Firebase Console](https://console.firebase.google.com/)
   - Create a new project or select an existing one

2. **Enable Authentication Methods**
   - In Firebase Console, navigate to **Authentication** > **Sign-in method**
   - Enable **Email/Password** authentication
   - Enable **Google** authentication

3. **Get Firebase Configuration**
   - Go to **Project Settings** > **General**
   - Under "Your apps", find your Web API Key
   - Note down:
     - API Key
     - Project ID
     - Auth Domain (usually `[project-id].firebaseapp.com`)

4. **Configure Google OAuth (Optional)**
   - In Firebase Console under Authentication > Sign-in method > Google
   - Note the Web Client ID and Client Secret
   - Or create new credentials in Google Cloud Console

## Configuration

Update `appsettings.json` with your Firebase credentials:

```json
{
  "Firebase": {
    "ApiKey": "YOUR_FIREBASE_API_KEY",
    "AuthDomain": "YOUR_PROJECT_ID.firebaseapp.com",
    "ProjectId": "YOUR_PROJECT_ID"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

**Important:** For production, use User Secrets or environment variables instead of hardcoding credentials.

### Using User Secrets (Recommended for Development)

```bash
dotnet user-secrets init
dotnet user-secrets set "Firebase:ApiKey" "your-api-key"
dotnet user-secrets set "Firebase:AuthDomain" "your-auth-domain"
dotnet user-secrets set "Firebase:ProjectId" "your-project-id"
dotnet user-secrets set "Authentication:Google:ClientId" "your-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-client-secret"
```

## Running the Application

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Build the application:**
   ```bash
   dotnet build
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Open your browser:**
   Navigate to `https://localhost:5001` or `http://localhost:5000`

## Project Structure

```
├── Models/
│   ├── FirebaseConfig.cs      # Firebase configuration model
│   ├── LoginModel.cs           # Login form model
│   └── RegisterModel.cs        # Registration form model
├── Pages/
│   ├── Index.cshtml            # Home page
│   ├── Login.cshtml            # Custom login page
│   ├── Login.cshtml.cs         # Login page logic
│   ├── Register.cshtml         # Custom register page
│   ├── Register.cshtml.cs      # Register page logic
│   ├── Logout.cshtml           # Logout page
│   └── Shared/
│       └── _Layout.cshtml      # Main layout with nav
├── Services/
│   └── FirebaseAuthService.cs  # Firebase authentication service
├── wwwroot/
│   └── css/
│       └── site.css            # Custom styles
├── Program.cs                  # App configuration
└── appsettings.json           # Configuration file
```

## Features Explained

### Authentication Flow

1. **Registration (`/register`)**
   - User provides email, password, and optional display name
   - Firebase creates the user account
   - User is automatically logged in with cookie authentication

2. **Login (`/login`)**
   - Email/Password login using Firebase SDK
   - Google OAuth login option
   - "Remember me" functionality
   - Session persisted in cookies

3. **Logout (`/logout`)**
   - Clears authentication cookie
   - Redirects to home page

### Security Features

- Cookie-based authentication with configurable expiration
- HTTPS redirection
- HSTS enabled in production
- Password validation (minimum 6 characters)
- Email validation
- CSRF protection with anti-forgery tokens

## Customization

### Styling

All custom styles are in `wwwroot/css/site.css`. The application uses:
- Gradient purple theme
- Card-based layouts
- Bootstrap 5 components
- Custom form controls
- Responsive design

### Authentication Settings

Modify in `Program.cs`:
- Cookie expiration time (default: 30 minutes)
- Sliding expiration
- Login/Logout paths
- Authentication schemes

## Troubleshooting

### Firebase Authentication Errors

- **"Registration failed"**: Check Firebase API key and ensure Email/Password auth is enabled
- **"Sign in failed"**: Verify credentials and Firebase configuration
- **Google login not working**: Ensure Google OAuth is configured in both Firebase and Google Cloud Console

### Build Errors

- Run `dotnet restore` to ensure all packages are installed
- Check that .NET 8.0 SDK is installed: `dotnet --version`

## Technologies Used

- .NET 8.0
- ASP.NET Core Razor Pages
- Firebase Authentication SDK (FirebaseAuthentication.net 4.1.0)
- Microsoft.AspNetCore.Authentication.OpenIdConnect
- Microsoft.AspNetCore.Authentication.Google
- Bootstrap 5
- Cookie Authentication

## License

This project is provided as-is for educational purposes.

## Contributing

Feel free to submit issues or pull requests for improvements!
