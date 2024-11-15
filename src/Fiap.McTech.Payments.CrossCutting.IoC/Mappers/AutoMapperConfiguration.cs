using AutoMapper;
using Fiap.McTech.Payments.Application.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.McTech.Payments.CrossCutting.IoC.Mappers
{
    public static class AutoMapperConfiguration
    {
        public static void RegisterMappings(this IServiceCollection services)
        {
            MapperConfiguration config = new(cfg =>
            {
                // Register Profiles
                cfg.CreateMap<Fiap.McTech.Payments.Domain.Entities.Payments, PaymentOutputDto>();
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
