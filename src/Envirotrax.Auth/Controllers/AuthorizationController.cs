
using System.Security.Claims;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Services.Definitions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Envirotrax.Auth.Areas.OpenIdConnect.Controllers
{
    [ApiController]
    [Route("oauth/connect")]
    public class AuthorizationController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;

        public AuthorizationController(
            SignInManager<AppUser> signInManager,
            IAuthService authService)
        {
            _signInManager = signInManager;
            _authService = authService;
        }

        private int? GetIntegerFromAcr(string? acrValues, string parameterName)
        {
            var param = AcrHelper.ParseAcrValues(acrValues)
                .SingleOrDefault(value => value.ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase));

            if (param != null)
            {
                if (int.TryParse(param.ParameterValue, out var intValue))
                {
                    return intValue;
                }
            }

            return default;
        }

        private int? GetWaterSupplierId(string? acrValues)
        {
            return GetIntegerFromAcr(acrValues, "waterSupplierId");
        }

        private int? GetContractorId(string? acrValues)
        {
            return GetIntegerFromAcr(acrValues, "contractorId");
        }

        private int GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.FindFirstValue(OpenIddictConstants.Claims.Subject);

            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return 0;
        }

        private async Task<IActionResult> HandleAuthCodeGrantTypeAsync(OpenIddictRequest request)
        {
            // Retrieve the claims principal stored in the authorization code
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal!;

            claimsPrincipal.SetAudiences(request.ClientId!);

            await _authService.SetSecurityPropertiesAsync(
                principal: claimsPrincipal,
                userId: GetUserId(claimsPrincipal),
                waterSupplierId: GetWaterSupplierId(request.AcrValues),
                contractorId: GetContractorId(request.AcrValues)
            );

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private async Task<IActionResult> HandleRefreshTokenAsync(OpenIddictRequest request)
        {
            // Retrieve the claims principal stored in the authorization code
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal!;

            claimsPrincipal.SetAudiences(request.ClientId!);

            await _authService.SetSecurityPropertiesAsync(
                principal: claimsPrincipal,
                userId: GetUserId(claimsPrincipal),
                waterSupplierId: GetWaterSupplierId(request.AcrValues),
                contractorId: GetContractorId(request.AcrValues)
            );

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private IActionResult HandleClientCredentials(OpenIddictRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ClientId))
            {
                throw new InvalidOperationException();
            }

            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Subject (sub) is a required field, we use the client id as the subject identifier here.
            identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

            var claimsPrincipal = new ClaimsPrincipal(identity);

            claimsPrincipal.SetScopes(request.GetScopes());
            claimsPrincipal.SetAudiences(request.ClientId);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("token")]
        public async Task<IActionResult> ExchangeAsync()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsAuthorizationCodeGrantType())
            {
                return await HandleAuthCodeGrantTypeAsync(request);
            }

            if (request.IsRefreshTokenGrantType())
            {
                return await HandleRefreshTokenAsync(request);
            }

            if (request.IsClientCredentialsGrantType())
            {
                return HandleClientCredentials(request);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        [HttpGet("userinfo")]
        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        public IActionResult GetUser()
        {
            var claimGroup = User
                .Claims
                .GroupBy(claim => claim.Type);

            var result = new Dictionary<string, object?>();

            foreach (var group in claimGroup)
            {
                object? value = group.Count() > 1
                    ? group.Select(claim => claim.Value)
                    : group.FirstOrDefault()?.Value;

                result.Add(group.Key, value);
            }

            return Ok(result);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();

            return SignOut(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    // .NET 8 requires MVC controllers to be completely separate from API controllers
    // The authroize challenge will return 401 instead of 302 if this is not separate.
    [Route("oauth/connect")]
    public class AuthorizeMvcController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public AuthorizeMvcController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        private string GetReturnUrl()
        {
            return Request.PathBase + Request.Path + QueryString.Create(
                Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList());
        }

        private ChallengeResult Challenge(IEnumerable<AcrValue> parsedAcrValues)
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = GetReturnUrl()
            };

            return Challenge(authProperties);
        }

        private async Task<IActionResult> ChallengeAsync(string? acrValues)
        {
            var parsedAcrValues = AcrHelper.ParseAcrValues(acrValues);

            var externalAuthProvider = parsedAcrValues
                .SingleOrDefault(value => value.ParameterName.Equals("provider", StringComparison.OrdinalIgnoreCase));

            if (externalAuthProvider != null)
            {
                var registeredExternalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();

                var matchingProvider = registeredExternalProviders.FirstOrDefault(provider => provider.Name.Equals(externalAuthProvider.ParameterValue, StringComparison.OrdinalIgnoreCase));

                if (matchingProvider != null)
                {
                    var redirectUrl = Url.Page("/Account/ExternalLogin", pageHandler: "Callback", values: new { area = "Identity", returnUrl = GetReturnUrl() });
                    var properties = _signInManager.ConfigureExternalAuthenticationProperties(matchingProvider.Name, redirectUrl);

                    return Challenge(
                        authenticationSchemes: matchingProvider.Name,
                        properties: properties
                    );
                }
            }

            return Challenge(parsedAcrValues);
        }

        [HttpGet("authorize")]
        [HttpPost("authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the user principal stored in the authentication cookie.
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            // If the user principal can't be extracted, redirect the user to the login page.
            if (!result.Succeeded)
            {
                return await ChallengeAsync(request.AcrValues);
            }

            // Create a new claims principal
            var claims = new List<Claim>
            {
                // 'subject' claim which is required
                new Claim(OpenIddictConstants.Claims.Subject, result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!),
                new Claim(OpenIddictConstants.Claims.Name, result.Principal.FindFirstValue(ClaimTypes.Name)!).SetDestinations(OpenIddictConstants.Destinations.AccessToken),
                new Claim(OpenIddictConstants.Claims.Email, result.Principal.FindFirstValue(ClaimTypes.Email)!).SetDestinations(OpenIddictConstants.Destinations.AccessToken)
            };

            if (result.Principal.HasClaim(ClaimTypes.GivenName))
            {
                claims.Add(new Claim(OpenIddictConstants.Claims.GivenName, result.Principal.FindFirstValue(ClaimTypes.GivenName)!).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }

            if (result.Principal.HasClaim(ClaimTypes.Surname))
            {
                claims.Add(new Claim(OpenIddictConstants.Claims.FamilyName, result.Principal.FindFirstValue(ClaimTypes.Surname)!).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set requested scopes (this is not done automatically)
            claimsPrincipal.SetScopes(request.GetScopes());

            // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    static class AcrHelper
    {
        public static IEnumerable<AcrValue> ParseAcrValues(string? acrValues)
        {
            if (!string.IsNullOrWhiteSpace(acrValues))
            {
                return acrValues
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(values =>
                    {
                        var keyPairs = values.Split(':');

                        return new AcrValue
                        {
                            ParameterName = keyPairs[0],
                            ParameterValue = keyPairs[1]
                        };
                    });
            }

            return [];
        }
    }

    class AcrValue
    {
        public string ParameterName { get; set; } = null!;
        public string ParameterValue { get; set; } = null!;
    }
}