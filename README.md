# .NET 8 Razor Pages Firebase Authentication

A complete .NET 8 Razor Pages application demonstrating Firebase authentication using Firebase Web SDK and FirebaseUI with server-side token verification.

## Features

- ✅ Firebase Web SDK integration (client-side authentication)
- ✅ FirebaseUI for authentication UI
- ✅ Email/Password authentication
- ✅ Google OAuth authentication (via Firebase)
- ✅ Server-side Firebase ID token verification
- ✅ Custom CSS styling
- ✅ Session management with cookies
- ✅ User-friendly UI with gradient design
- ✅ Responsive layout

## Prerequisites

- .NET 8.0 SDK or later
- Firebase account
- Firebase project with Authentication enabled

## Firebase Setup

1. **Create a Firebase Project**
   - Go to [Firebase Console](https://console.firebase.google.com/)
   - Create a new project or select an existing one

2. **Enable Authentication Methods**
   - In Firebase Console, navigate to **Authentication** > **Sign-in method**
   - Enable **Email/Password** authentication
   - Enable **Google** authentication (optional)

3. **Get Firebase Configuration**
   - Go to **Project Settings** > **General**
   - Under "Your apps", add a Web app or select existing one
   - Note down:
     - API Key
     - Project ID
     - Auth Domain (usually `[project-id].firebaseapp.com`)

## Configuration

Update `appsettings.json` with your Firebase credentials:

```json
{
  "Firebase": {
    "ApiKey": "YOUR_FIREBASE_API_KEY",
    "AuthDomain": "YOUR_PROJECT_ID.firebaseapp.com",
    "ProjectId": "YOUR_PROJECT_ID"
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
│   ├── Login.cshtml            # Login page with FirebaseUI
│   ├── Login.cshtml.cs         # Login page logic with token verification
│   ├── Register.cshtml         # Register page with FirebaseUI
│   ├── Register.cshtml.cs      # Register page logic with token verification
│   ├── Logout.cshtml           # Logout page
│   └── Shared/
│       └── _Layout.cshtml      # Main layout with nav
├── Services/
│   └── FirebaseAuthService.cs  # Firebase token verification service
├── wwwroot/
│   └── css/
│       └── site.css            # Custom styles
├── Program.cs                  # App configuration
└── appsettings.json           # Configuration file
```

## Features Explained

### Authentication Flow

1. **Registration (`/register`)**
   - User provides email, password, and display name via FirebaseUI
   - Firebase Web SDK creates the user account client-side
   - ID token is sent to server for verification
   - Server creates a cookie-based session

2. **Login (`/login`)**
   - Email/Password or Google OAuth login via FirebaseUI
   - Firebase Web SDK handles authentication client-side
   - ID token is sent to server for verification
   - Session persisted in cookies

3. **Logout (`/logout`)**
   - Clears authentication cookie
   - Redirects to home page

### Security Features

- **Firebase ID Token Verification**: All authentication tokens are verified server-side using Firebase Admin SDK
- Cookie-based authentication with configurable expiration
- HTTPS redirection
- HSTS enabled in production
- Client-side authentication with server-side verification
- No credentials stored client-side

## Technology Stack

### Client-Side
- Firebase Web SDK v10.7.1 (via CDN)
- FirebaseUI v6.1.0 (via CDN)
- Bootstrap 5
- Vanilla JavaScript

### Server-Side
- .NET 8.0
- ASP.NET Core Razor Pages
- Firebase Admin SDK 3.0.0
- Cookie Authentication

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

### FirebaseUI Configuration

Modify in `Login.cshtml` and `Register.cshtml`:
- Sign-in providers
- UI customization
- Callback URLs
- Terms of Service / Privacy Policy links

## Troubleshooting

### Firebase Authentication Errors

- **"Invalid ID token"**: Check Firebase project configuration and ensure API key and project ID are correct
- **"Unable to get user"**: Verify Firebase Admin SDK initialization and project ID
- **Google login not working**: Ensure Google OAuth is enabled in Firebase Console

### Build Errors

- Run `dotnet restore` to ensure all packages are installed
- Check that .NET 8.0 SDK is installed: `dotnet --version`

## Differences from Traditional Approach

This implementation uses Firebase Web SDK (client-side) with FirebaseUI instead of server-side authentication libraries:

**Benefits:**
- Better user experience with FirebaseUI's polished UI
- Automatic handling of OAuth flows
- Built-in error handling and validation
- Consistent authentication across platforms
- Reduced server-side code complexity
- Official Firebase recommended approach

## License

This project is provided as-is for educational purposes.

## Contributing

Feel free to submit issues or pull requests for improvements!
