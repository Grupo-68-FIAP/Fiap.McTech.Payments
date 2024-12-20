﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.McTech.Payments.Presentation.API.Configurations
{
    /// <summary>
    /// OpenId configuration class.
    /// </summary>
    [ExcludeFromCodeCoverage] 
    public static class OpenIdConfig
    {
        /// <summary>
        /// Add OpenIdConnect configuration to the application.
        /// </summary>
        /// <param name="services">ServiceCollection instance.</param>
        public static void AddJwtBearerAuthentication(this IServiceCollection services)
        {
            string openIdAuthority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY")
                ?? throw new MissingFieldException("Need to configure a Open Id Authority and Audiance");

            var clientId = Environment.GetEnvironmentVariable("OPENID_AUDIENCE");


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = openIdAuthority;
                    options.Audience = clientId;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = openIdAuthority,
                        ValidAudience = clientId,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    };
                });
        }

        /// <summary>
        /// Add OpenIdConnect configuration to the application.
        /// </summary>
        /// <param name="app">WebApplication instance.</param>
        public static void UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
