using Duende.IdentityServer.Models;
using Duende.IdentityServer;

namespace ApiMicrosservicesIdentityServer.Identity.Configuration;

public class IdentityConfiguration
{
    public const string Admin = "Admin";
    public const string Client = "Client";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
                new("apiMicrosservices", "ApiMicrosservices Server"),
                new(name: "read", "Read data."),
                new(name: "write", "Write data."),
                new(name: "delete", "Delete data."),
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
                new() {
                    ClientId = "client",
                    ClientSecrets = { new Secret("!77k@x/waxm=;4`9.@3%cjff%7zqbtw)".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials, 
                    AllowedScopes = {"read", "write", "profile" }
                },
                new() {
                    ClientId = "apiMicrosservices",
                    ClientSecrets = { new Secret("!77k@x/waxm=;4`9.@3%cjff%7zqbtw)".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {"https://localhost:7157/signin-oidc"},
                    PostLogoutRedirectUris = {"https://localhost:7157/signout-callback-oidc"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "apiMicrosservices"
                    }
                }
        };
}
