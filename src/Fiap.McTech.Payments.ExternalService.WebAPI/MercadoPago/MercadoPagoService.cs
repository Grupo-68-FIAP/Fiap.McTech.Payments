using Fiap.McTech.Payments.Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Fiap.McTech.Payments.ExternalService.WebAPI.MercadoPago
{
    [ExcludeFromCodeCoverage]
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly ILogger<MercadoPagoService> _logger;
        private readonly HttpClient _httpClient;

        public MercadoPagoService(
            ILogger<MercadoPagoService> logger,
            HttpClient httpClient
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GeneratePaymentLinkAsync(PaymentRequest request)
        {
            try
            {
                string json = JsonSerializer.Serialize(request);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("v1/payments", content);

                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseString);

                return paymentResponse?.PointOfInteraction?.TransactionData?.TicketUrl ?? throw new InvalidOperationException("Falha ao recuperar o QR code.");
            }
            catch (InvalidOperationException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error while generating payment link for the amount {Amount}.", request.TransactionAmount);
                throw new InvalidOperationException("There was a problem communicating with the payment service.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate payment link for the amount {Amount}.", request.TransactionAmount);
                return string.Empty;
            }
        }

        public async Task<string> GenerateMockPaymentLinkAsync(PaymentRequest request)
        {
            try
            {
                HttpResponseMessage response = mockPaymentResponse();

                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseString);

                return paymentResponse?.PointOfInteraction?.TransactionData?.TicketUrl ?? throw new InvalidOperationException("Falha ao recuperar o QR code.");
            }
            catch (InvalidOperationException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error while generating payment link for the amount {Amount}.", request.TransactionAmount);
                throw new InvalidOperationException("There was a problem communicating with the payment service.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate payment link for the amount {Amount}.", request.TransactionAmount);
                return string.Empty;
            }
        }

        public async Task<bool> ProcessPaymentAsync(Guid paymentId)
        {
            try
            {
                _logger.LogInformation("Processing payment from paymentId: {paymentId}.", paymentId);

                //MOCK VALUE
                return await Task.Run(() => { return true; });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process payment from paymentId: {paymentId}.", paymentId);

                return await Task.Run(() => { return false; });
            }
        }

        private static HttpResponseMessage mockPaymentResponse()
        {
            var result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            result.Content = new StringContent(@"
            {
                ""point_of_interaction"": {
                    ""type"": ""PIX"",
                    ""application_data"": {
                    ""name"": ""NAME_SDK"",
                    ""version"": ""VERSION_NUMBER""
                    },
                    ""transaction_data"": {
                    ""qr_code_base64"": ""iVBORw0KGgoAAAANSUhEUgAABRQAAAUUCAYAAACu5p7oAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAIABJREFUeJzs2luO3LiWQNFmI+Y/Zd6vRt36KGNXi7ZOBtcagHD4kNLeiLX33v8DAAAAABD879sDAAAAAAA/h6AIAAAAAGSCIgAAAACQCYoAAAAAQCYoAgAAAACZoAgAAAAAZIIiAAAAAJAJigAAAABAJigCAAAAAJmgCAAAAABkgiIAAAAAkAmKAAAAAEAmKAIAAAAAmaAIAAAAAGSCIgAAAACQCYoAAAAAQCYoAgAAAACZoAgAAAAAZIIiAAAAAJAJigAAAABAJigCA..."",
                    ""qr_code"": ""00020126600014br.gov.bcb.pix0117test@testuser.com0217dados adicionais520400005303986540510.005802BR5913Maria Silva6008Brasilia62070503***6304E2CA"",
                    ""ticket_url"": ""https://www.mercadopago.com.br/payments/123456789/ticket?caller_id=123456&hash=123e4567-e89b-12d3-a456-426655440000""
                    }
                }
            }");
            return result;
        }
    }
}
