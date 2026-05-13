# Mma.Nafath.Web

A standalone .NET library for **Nafath Web Redirect** authentication. This library provides a reusable service to interact with the Nafath API, create authentication sessions, and decrypt JWE tokens received from the redirect.

## Features

- **Session Creation**: Easily create authentication sessions with Nafath.
- **Token Decryption**: Strong-typed decryption of JWE tokens using certificates.
- **Multi-Framework Support**: Targets .NET 8, .NET 9, and .NET 10.
- **Strongly Typed Configuration**: Simple setup via `IOptions`.

## Installation

```bash
dotnet add package Mma.Nafath.Web
```

## Setup

Add Nafath authentication to your `IServiceCollection`:

```csharp
services.AddNafathWebAuthentication(options =>
{
    options.BaseUrl = "https://api.nafath.sa";
    options.AppId = "your-app-id";
    options.AppKey = "your-app-key";
    options.CertificatePath = "path/to/your/certificate.pfx";
    options.CertificatePassword = "your-password";
    options.UseCertificate = true;
});
```

## Usage

### Creating a Session

```csharp
public class MyService
{
    private readonly INafathWebService _nafathService;

    public MyService(INafathWebService nafathService)
    {
        _nafathService = nafathService;
    }

    public async Task<NafathSessionResponse> StartLogin(string nationalId)
    {
        return await _nafathService.CreateSessionAsync(nationalId, "trans-id");
    }
}
```

### Decrypting a Token

```csharp
var decryptedPayload = _nafathService.DecryptToken(jweToken);
```

## License

This project is licensed under the MIT License.
