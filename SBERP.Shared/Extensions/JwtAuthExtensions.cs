using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SBERP.Shared.Models;
using SBERP.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SBERP.Shared.Extensions
{
    public static class JwtAuthExtensions
    {
        /// <summary>
        /// Call in every microservice's Program.cs:
        ///
        ///   builder.Services.AddSharedJwtAuthentication(builder.Configuration);
        ///
        /// Reads "SharedJwtSettings" from appsettings.json and configures
        /// JwtBearer with the same Key/Issuer/Audience as SBERP.Security.
        /// Also registers ITokenBlacklistService for the Redis blacklist check.
        /// </summary>
        public static IServiceCollection AddSharedJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection("SharedJwtSettings")
                .Get<JwtSharedSettings>();

            if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Key))
                throw new InvalidOperationException(
                    "SharedJwtSettings is missing or incomplete in appsettings.json. " +
                    "Key, Issuer and Audience must match SBERP.Security AppSettings:JWT exactly.");

            var keyBytes = Encoding.ASCII.GetBytes(jwtSettings.Key);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(keyBytes),
                        // Zero tolerance — reject tokens even 1 second past expiry
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        // Angular auth.req.interceptor.ts watches "Token-Expired" header
                        // to trigger the refresh token flow automatically
                        OnAuthenticationFailed = ctx =>
                        {
                            if (ctx.Exception is SecurityTokenExpiredException)
                                ctx.Response.Headers.Append(
                                    "Token-Expired", "true");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();

            // Register the Redis blacklist service — used by TokenBlacklistMiddleware
            services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

            return services;
        }
    }
}
