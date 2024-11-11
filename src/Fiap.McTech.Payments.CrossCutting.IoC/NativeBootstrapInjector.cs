using Fiap.McTech.Application.AppServices.Payment;
using Fiap.McTech.Payments.Application.Interfaces;
using Fiap.McTech.Payments.CrossCutting.IoC.Infra.Context;
using Fiap.McTech.Payments.Domain.Interfaces.ExternalServices;
using Fiap.McTech.Payments.ExternalService.WebAPI.MercadoPago;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Fiap.McTech.Payments.CrossCutting.IoC
{
    public static class NativeBootstrapInjector
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Infra 
            services.ConfigureSqlServer(configuration);
            services.RegisterRepositories();

            //SERVICES
            services.AddScoped<IMercadoPagoService, MercadoPagoService>();

            services.AddHttpClient<IMercadoPagoService, MercadoPagoService>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(configuration["MercadoPagoConfig:BaseUrl"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["MercadoPagoConfig:AccessToken"]);
                client.DefaultRequestHeaders.Add("X-Idempotency-Key", configuration["MercadoPagoConfig:IdempotencyKey"]);
            });

            //APP Services
            services.AddScoped<IPaymentsAppService, PaymentsAppService>();
        }
    }
}
