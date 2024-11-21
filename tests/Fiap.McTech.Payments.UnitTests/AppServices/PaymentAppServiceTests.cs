using Fiap.McTech.Application.AppServices.Payment;
using Fiap.McTech.Payments.Application.Dtos;
using Fiap.McTech.Payments.CrossCutting.Exceptions;
using Fiap.McTech.Payments.Domain.Enums;
using Fiap.McTech.Payments.Domain.Interfaces.ExternalServices;
using Fiap.McTech.Payments.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.McTech.UnitTests.AppServices
{
    public class PaymentAppServiceTests
    {
        private readonly Mock<ILogger<PaymentsAppService>> _mockLogger;
        private readonly Mock<IPaymentsRepository> _mockPaymentRepository;
        private readonly Mock<IMercadoPagoService> _mockMercadoPagoService;
        private readonly PaymentsAppService _paymentAppService;

        public PaymentAppServiceTests()
        {
            _mockLogger = new Mock<ILogger<PaymentsAppService>>();
            _mockPaymentRepository = new Mock<IPaymentsRepository>();
            _mockMercadoPagoService = new Mock<IMercadoPagoService>();

            _paymentAppService = new PaymentsAppService(
                _mockLogger.Object,
                _mockPaymentRepository.Object,
                _mockMercadoPagoService.Object
            );
        }

        [Fact]
        public async Task GenerateQRCodeAsync_ReturnsSuccessResult_WhenQRCodeIsGenerated()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var model = new PaymentInputDto
            {
                OrderId = orderId,
                TotalAmount = 100.0m,
                Client = new PaymentInputDto.ClientDto
                {
                    Cpf = "12345678909",
                    Email = "john.doe@example.com",
                    Name = "Teste",
                    Id = Guid.NewGuid()
                }
            };

            _mockMercadoPagoService
                .Setup(service => service.GeneratePaymentLinkAsync(It.IsAny<PaymentRequest>()))
                .ReturnsAsync("https://example.com/qrcode");

            _mockPaymentRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Fiap.McTech.Payments.Domain.Entities.Payments>()))
                .ReturnsAsync(
                    new Fiap.McTech.Payments.Domain.Entities.Payments(
                        model.Client.Id,
                        model.OrderId,
                        model.TotalAmount,
                        "ClientName",
                        "client@example.com",
                        PaymentMethod.QrCode,
                        PaymentStatus.Pending
                    )
                );

            // Act
            var result = await _paymentAppService.GenerateQRCodeAsync(orderId, model);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("QR code generated successfully.", result.Message);
            Assert.NotEqual(Guid.Empty, result.PaymentId);
            Assert.Equal("https://example.com/qrcode", result.QRCode);
        }

        [Fact]
        public async Task GenerateQRCodeAsync_ThrowsException_WhenOrderIdDiffersFromModel()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<PaymentRequiredException>(() => _paymentAppService.GenerateQRCodeAsync(orderId, new PaymentInputDto()));
        }

        [Fact]
        public async Task UpdatePayment_ReturnsSuccessResult_WhenPaymentIsProcessedSuccessfully()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var payment = new Fiap.McTech.Payments.Domain.Entities.Payments(Guid.NewGuid(), Guid.NewGuid(), 100.0m, "ClientName", "client@example.com", PaymentMethod.QrCode, PaymentStatus.Pending);
            _mockPaymentRepository.Setup(repo => repo.GetByIdAsync(paymentId)).ReturnsAsync(payment);
            _mockMercadoPagoService.Setup(service => service.ProcessPaymentAsync(paymentId)).ReturnsAsync(true);

            // Act
            var result = await _paymentAppService.UpdatePayment(paymentId, "Completo");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Payment processed successfully.", result.Message);
            _mockPaymentRepository.Verify(repo => repo.UpdateAsync(payment), Times.Once);
        }

        [Fact]
        public async Task UpdatePayment_ReturnsFailureResult_WhenPaymentNotFound()
        {
            // Arrange
            var paymentId = Guid.NewGuid();

            // Act
            var result = await _paymentAppService.UpdatePayment(paymentId, "Completo");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Payment Not Found.", result.Message);
            _mockPaymentRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Fiap.McTech.Payments.Domain.Entities.Payments>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePayment_ReturnsFailureResult_WhenPaymentProcessingFails()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var payment = new Fiap.McTech.Payments.Domain.Entities.Payments(Guid.NewGuid(), Guid.NewGuid(), 100.0m, "ClientName", "client@example.com", PaymentMethod.QrCode, PaymentStatus.Pending);
            _mockPaymentRepository.Setup(repo => repo.GetByIdAsync(paymentId)).ReturnsAsync(payment);
            _mockMercadoPagoService.Setup(service => service.ProcessPaymentAsync(paymentId)).ReturnsAsync(false);

            // Act
            var result = await _paymentAppService.UpdatePayment(paymentId, "Completo");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Payment failed.", result.Message);
            _mockPaymentRepository.Verify(repo => repo.UpdateAsync(payment), Times.Never);
        }
    }
}
