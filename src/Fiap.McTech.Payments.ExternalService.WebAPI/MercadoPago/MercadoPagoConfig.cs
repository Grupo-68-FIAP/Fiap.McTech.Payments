using System.Diagnostics.CodeAnalysis;

namespace Fiap.McTech.Payments.ExternalService.WebAPI.MercadoPago
{
    [ExcludeFromCodeCoverage]
    public class MercadoPagoConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}
