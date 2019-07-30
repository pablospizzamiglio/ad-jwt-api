using ad_jwt_api.Entities;
using ad_jwt_api.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace ad_jwt_api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("WorksInArgentina", policy =>
                    policy.Requirements.Add(new CountryRequirement("AR"))
                );
            });
            services.AddSingleton<IAuthorizationHandler, CountryHandler>();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            JwtOptions jwtOptions = configuration.GetSection("JwtAuthentication").Get<JwtOptions>();
            services.Configure<JwtOptions>(configuration.GetSection("JwtAuthentication"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(options =>
            {
                options.ClaimsIssuer = jwtOptions.ValidIssuer;
                options.IncludeErrorDetails = true;
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = jwtOptions.SymmetricSecurityKey,
                    ValidateActor = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.ValidIssuer,
                    ValidAudience = jwtOptions.ValidAudience
                };
            });
        }
    }
}
