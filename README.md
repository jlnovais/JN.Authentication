# JN.Authentication

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Nuget](https://img.shields.io/nuget/v/JN.Authentication)](https://www.nuget.org/packages/JN.Authentication/) [![Build Status](https://travis-ci.com/jlnovais/JN.Authentication.svg?branch=master)](https://travis-ci.com/jlnovais/JN.Authentication) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/dc321d37d5f3495992531071fb43aa5f)](https://app.codacy.com/manual/jlnovais/JN.Authentication?utm_source=github.com&utm_medium=referral&utm_content=jlnovais/JN.Authentication&utm_campaign=Badge_Grade_Dashboard)

Simple Authentication implementation for ASP.NET Core.

*   Basic Authentication Scheme
*   API Key Custom Authentication Scheme

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
          options.ChallengeResponse = ValidationService.ChallengeResponseBasic;
      });

    // validation service
    services.AddSingleton<IBasicValidationService, BasicValidationService>();

    // ApiKey authentication
    services.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
      .AddApiKey(options =>
      {
          options.LogInformation = true;
          options.HttpPostMethodOnly = false;
          options.AcceptsQueryString = true;
          options.HeaderName = "ApiKey";
          options.ChallengeResponse = ValidationService.ChallengeResponseApikey;
      });

    // validation service
    services.AddSingleton<IApiKeyValidationService, ApiKeyValidationService>();

}
```
`ChallengeResponseBasic` and `ChallengeResponseApikey` are delegates called before a 401 response is sent to the client.

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

*   `LogInformation`: log information using a logging provider 
*   `HttpPostMethodOnly`: allows only POST requests

Basic allows to specify a `Realm` and `HeaderEncoding`.

ApiKey authentication allows to change the `HeaderName` (default is "ApiKey") and can also accept the key in the query string (`AcceptsQueryString`)
