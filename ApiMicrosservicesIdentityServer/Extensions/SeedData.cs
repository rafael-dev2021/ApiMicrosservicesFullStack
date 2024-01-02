using ApiMicrosservicesIdentityServer.Identity.Interfaces;

namespace ApiMicrosservicesIdentityServer.Extensions;

public static class SeedData
{
    public static async Task SeedUsersRoles(IApplicationBuilder builder)
    {
        var scope = builder.ApplicationServices.CreateScope();
        var result = scope.ServiceProvider.GetService<ISeedRoleAndUser>();

        await result.SeedRoleAsync();
        await result.SeedUserAsync();
    }
}
