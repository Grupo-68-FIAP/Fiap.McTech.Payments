using Fiap.McTech.Payments.Application.Dtos;
using Fiap.McTech.Payments.Application.Interfaces;
using Fiap.McTech.Payments.Application.Services.Mappers;
using Fiap.McTech.Payments.CrossCutting.Exceptions;
using Fiap.McTech.Payments.Domain.Enums;
using Fiap.McTech.Payments.Domain.Interfaces.ExternalServices;
using Fiap.McTech.Payments.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Fiap.McTech.Payments.CrossCutting.Extensions;

namespace Fiap.McTech.Application.AppServices.Payment
{
    /// <summary>
    /// Represents an application service for managing payments.
    /// </summary>
    public class PaymentsAppService : IPaymentsAppService
    {
        private readonly ILogger<PaymentsAppService> _logger;
        private readonly IPaymentsRepository _paymentRepository;
        private readonly IMercadoPagoService _mercadoPagoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAppService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="paymentRepository">The repository for managing payments.</param>
        /// <param name="orderRepository">The repository for managing orders.</param>
        /// <param name="mercadoPagoService">The Mercado Pago payment service.</param>
        public PaymentsAppService(
            ILogger<PaymentsAppService> logger,
            IPaymentsRepository paymentRepository,
            IMercadoPagoService mercadoPagoService)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
            _mercadoPagoService = mercadoPagoService;
        }

        /// <inheritdoc/>
        public async Task<GenerateQRCodeResultDto> GenerateQRCodeAsync(Guid orderId, PaymentInputDto model)
        {
            if (model == default)
                throw new ArgumentNullException(nameof(model));

            if (orderId != model.OrderId)
                throw new PaymentRequiredException("Invalid OrderId value sent to controller");

            _logger.LogInformation("Generating QR code for order with ID {OrderId}.", model.OrderId);

            var paymentLink = await _mercadoPagoService.GeneratePaymentLinkAsync(model.MapPaymentToServiceModel());
            if (string.IsNullOrEmpty(paymentLink))
            {
                _logger.LogInformation("Error to create QrCode for ID {OrderId}.", model.OrderId);
                throw new InvalidOperationException($"Error to create QrCode for ID {model.OrderId}.");
            }

            var payment = await _paymentRepository.AddAsync(new Payments.Domain.Entities.Payments(
                model.Client.Id,
                model.OrderId,
                model.TotalAmount,
                model.Client?.Name ?? string.Empty,
                model.Client?.Email.ToString() ?? string.Empty,
                PaymentMethod.QrCode,
                PaymentStatus.Pending
            ));

            return new GenerateQRCodeResultDto(
                success: true,
                message: "QR code generated successfully.",
                payment.Id,
                qrCode: paymentLink
            );
        }

        /// <inheritdoc/>
        public async Task<PaymentOutputDto> UpdatePayment(Guid paymentId, string status)
        {
            _logger.LogInformation("Processing payment for order with ID {PaymentId}.", paymentId);

            var statusEnum = EnumExtensions.GetFromDescription(status, typeof(PaymentStatus));

            if (statusEnum == -1)
            {
                throw new EntityValidationException("Invalid Payment Status");
            }

            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogInformation("Payment with ID {OrderId} not found.", paymentId);
                return new PaymentOutputDto(success: false, message: "Payment Not Found.");
            }

            var paymentResult = await _mercadoPagoService.ProcessPaymentAsync(paymentId);
            if (!paymentResult)
            {
                _logger.LogInformation("Payment failed for order with ID {OrderId}.", paymentId);

                return new PaymentOutputDto(success: false, message: "Payment failed.");
            }

            payment.UpdateStatus(PaymentStatus.Completed);
            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation("Payment processed successfully for order with ID {OrderId}.", paymentId);

            return new PaymentOutputDto(success: true, message: "Payment processed successfully.");
        }
    }
}
