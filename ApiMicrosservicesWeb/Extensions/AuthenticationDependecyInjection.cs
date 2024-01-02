using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ApiMicrosservicesWeb.Extensions;

public static class AuthenticationDependecyInjection
{
    public static IServiceCollection AddAuthenticationDependecyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationBuilder()
        .AddPolicy("Admin", policy =>
        {
            policy.RequireRole("Admin");
        });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "oidc";
        }).AddCookie("Cookies", c =>
          {
              c.ExpireTimeSpan = TimeSpan.FromMinutes(10);
              c.Events = new CookieAuthenticationEvents()
              {
                  OnRedirectToAccessDenied = (context) =>
                  {
                      context.HttpContext.Response.Redirect(configuration["ServiceUri:IdentityServer"] + "/Account/AccessDenied");
                      return Task.CompletedTask;
                  }
              };
          }).AddOpenIdConnect("oidc", options =>
          {
              options.Events.OnRemoteFailure = context =>
              {
                  context.Response.Redirect("/");
                  context.HandleResponse();

                  return Task.FromResult(0);
              };

              options.Authority = configuration["ServiceUri:IdentityServer"];
              options.GetClaimsFromUserInfoEndpoint = true;
              options.ClientId = "apiMicrosservices";
              options.ClientSecret = configuration["Client:Secret"];
              options.ResponseType = "code";
              options.ClaimActions.MapJsonKey("role", "role", "role");
              options.ClaimActions.MapJsonKey("sub", "sub", "sub");
              options.TokenValidationParameters.NameClaimType = "name";
              options.TokenValidationParameters.RoleClaimType = "role";
              options.Scope.Add("apiMicrosservices");
              options.SaveTokens = true;
          });

        return services;
    }
}
