using ApiMicrosservicesIdentityServer.Identity.Interfaces;
using ApiMicrosservicesIdentityServer.Identity.Services;
using ApiMicrosservicesIdentityServer.Identity;
using Duende.IdentityServer.Services;

namespace ApiMicrosservicesIdentityServer.Extensions;
public static class ProfileAppServiceDependecyInjection
{
    public static IServiceCollection AddProfileAppServiceDependecyInjection(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ISeedRoleAndUser, SeedRoleAndUser>();
        services.AddScoped<IProfileService, ProfileAppService>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }
}
