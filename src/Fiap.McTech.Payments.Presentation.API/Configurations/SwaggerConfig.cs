﻿using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Fiap.McTech.Payments.Presentation.API.Configurations
{
    /// <summary>
    /// Swagger configuration class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class SwaggerConfig
    {
        /// <summary>
        /// Add Swagger configuration to the application.
        /// </summary>
        /// <param name="services">ServiceCollection instance.</param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                const string ApiVersion = "v1";
                c.SwaggerDoc(ApiVersion, new OpenApiInfo
                {
                    Title = "McTech Payments API",
                    Version = ApiVersion,
                    Description = "Backend API responsible for the payments of the McTech snack bar. It manages only functions about payments of the services offered by the McTech snack bar.\n\n" +
                                  "### Contacts\n" +
                                  "- **Ervin Notari Junior**: ervinnotari@hotmail.com\n" +
                                  "- **Guilherme Novaes da silva**: guilherme.novaes233@gmail.com\n" +
                                  "- **José Maria dos Reis Lisboa**: josemrlisboa@gmail.com\n" +
                                  "- **Vanessa Alves do Nascimento**: vanascimento.dev@gmail.com\n" +
                                  "- **Felipe Tomm**: felipe.tomm@gmail.com\n"
                });
                c.CustomSchemaIds(type => $"{type.FullName?.Replace($"{type.Namespace}.", "").Replace("+", ".")}");


                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insira 'Bearer' [espaço] e seu token JWT abaixo.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" }
                        },
                        Array.Empty<string>()
                    }
                });


                var xmlFiles = new List<string> {
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
                    "Fiap.McTech.Payments.Application.xml",
                    "Fiap.McTech.Payments.Domain.xml"
                };

                foreach (var xmlFile in xmlFiles)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
            });
        }

        /// <summary>
        /// Use Swagger configuration in the application.
        /// </summary>
        /// <param name="app">WebApplication instance.</param>
        public static void UseSwaggerV3(this WebApplication app)
        {
            app.UseSwagger();
            var allowSwaggerUi = Environment.GetEnvironmentVariable("ALLOW_SWAGGER_UI") == "true";
            if (app.Environment.IsDevelopment() || allowSwaggerUi)
            {
                app.UseSwaggerUI();
            }
        }
    }
}
