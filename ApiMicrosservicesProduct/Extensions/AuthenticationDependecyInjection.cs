﻿namespace ApiMicrosservicesProduct.Extensions;

public static class AuthenticationDependecyInjection
{
    public static IServiceCollection AddAuthenticationDependecyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddAuthorizationBuilder()
        //    .AddPolicy("Admin", policy =>
        //    {
        //        policy.RequireRole("Admin");
        //    });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

    //    services.AddSwaggerGen(c =>
    //    {
    //        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiMicrosservicesProduct", Version = "v1" });
    //        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //        {
    //            Description = @"'Bearer' [space] seu token",
    //            Name = "Authorization",
    //            In = ParameterLocation.Header,
    //            Type = SecuritySchemeType.ApiKey,
    //            Scheme = "Bearer"
    //        });

    //        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //     {
    //        new OpenApiSecurityScheme
    //        {
    //           Reference = new OpenApiReference
    //           {
    //              Type = ReferenceType.SecurityScheme,
    //              Id = "Bearer"
    //           },
    //           Scheme = "oauth2",
    //           Name = "Bearer",
    //           In= ParameterLocation.Header
    //        },
    //        new List<string> ()
    //     }
    //});
    //    });
    //    services.AddAuthentication("Bearer")
    //           .AddJwtBearer("Bearer", options =>
    //           {
    //               options.Authority =
    //                 configuration["ApiMicrosservicesIdentityServer:ApplicationUrl"];

    //               options.TokenValidationParameters = new TokenValidationParameters
    //               {
    //                   ValidateAudience = false
    //               };
    //           });

    //    services.AddAuthorization(options =>
    //    {
    //        options.AddPolicy("ApiScope", policy =>
    //        {
    //            policy.RequireAuthenticatedUser();
    //            policy.RequireClaim("scope", "apiMicrosservices");
    //        });
    //    });

        return services;
    }
}
