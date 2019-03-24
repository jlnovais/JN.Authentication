# JN.Authentication
Simple Authentication implementation for ASP.NET Core.

- Basic Authentication Scheme
- API Key Custom Authentication Scheme

## Install
Download the package from NuGet:

`Install-Package JN.Authentication`

## Usage
First, you must add one (or both) authentication scheme to the application pipeline:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Basic authentication 
    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
      .AddBasic(options =>
      {
          options.Realm = "api";
          options.LogInformation = true; //optional, default is false;
          options.HttpPostMethodOnly = false;
          options.HeaderEncoding = Encoding.UTF8; //optional, default is UTF8;
          options.ValidateUser = ValidateUser;
          options.ChallengeResponse = ChallengeResponseBasic;
      });

    // ApiKey authentication - using Scheme
    services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
      .AddApiKey(options =>
      {
          options.LogInformation = true;
          options.HttpPostMethodOnly = false;
          options.AcceptsQueryString = true;
          options.HeaderName = "ApiKey";
          options.ValidateKey = ValidateApiKey;
          options.ChallengeResponse = ChallengeResponseApikey;
      });
}
```
`ValidateUser`, `ChallengeResponseBasic`, `ValidateApiKey` and `ChallengeResponseApikey` are delegates used to validate access details (username/password or ApiKey). Example:

```csharp

public static Task<ChallengeResult> ChallengeResponse(Exception ex)
{
    // your code here...
}

public static Task<ValidationResult> ValidateApiKey(string key)
{
    // your code here...
}
```
On your controllers add the `Authorize` atribute and choose the Authentication Scheme ("Basic" or "ApiKey")

```csharp
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Basic", Policy = "IsAdminPolicy")]
[ApiController]

public class BasicAuthSchemeTestController : ControllerBase
{
   // Your code here
}
```
## Options

Both authentication schemes allows to:

- `LogInformation`: log information using a logging provider 
- `HttpPostMethodOnly`: allows only POST requests

Basic allows to specify a `Realm` and `HeaderEncoding`.

ApiKey authentication allows to change the `HeaderName` (default is "ApiKey") and can also accept the key in the query string (`AcceptsQueryString`)


