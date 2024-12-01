using System.Text.Json.Serialization;

namespace Fiap.McTech.Payments.Domain.Interfaces.ExternalServices
{
    public interface IMercadoPagoService
    {
        Task<string> GeneratePaymentLinkAsync(PaymentRequest request);
        Task<string> GenerateMockPaymentLinkAsync(PaymentRequest request);
        Task<bool> ProcessPaymentAsync(Guid paymentId);
    }

    public class PaymentRequest
    {
        [JsonPropertyName("transaction_amount")]
        public decimal TransactionAmount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("payment_method_id")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [JsonPropertyName("payer")]
        public Payer Payer { get; set; }
    }

    public class Payer
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("identification")]
        public Identification Identification { get; set; }
    }

    public class Identification
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("number")]
        public string Number { get; set; } = string.Empty;
    }
}
