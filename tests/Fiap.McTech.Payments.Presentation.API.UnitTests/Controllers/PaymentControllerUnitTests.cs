using Fiap.McTech.Payments.Presentation.API.Controllers;
using Fiap.McTech.Payments.Application.Dtos;
using Fiap.McTech.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.McTech.Api.UnitTests.Controllers
{
    public class PaymentControllerUnitTests
    {
        private readonly Mock<IPaymentsAppService> _mockPaymentAppService;
        private readonly PaymentController _controller;

        public PaymentControllerUnitTests()
        {
            _mockPaymentAppService = new Mock<IPaymentsAppService>();
            _controller = new PaymentController(_mockPaymentAppService.Object);
        }

        [Fact]
        public async Task GenerateQRCode_ReturnsOkResult_WithQrCodeUrl()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var expectedQrCodeUrl = "https://example.com/qrcode";
            var model = new PaymentInputDto
            {
                OrderId = paymentId,
                TotalAmount = 10,
                Client = new PaymentInputDto.ClientDto
                {
                    Cpf = "00000000019",
                    Email = "teste@teste.com",
                    Id = Guid.NewGuid(),
                    Name = "Teste"
                }
            };

            _mockPaymentAppService
                .Setup(service => service.GenerateQRCodeAsync(paymentId, model))
                .ReturnsAsync(new GenerateQRCodeResultDto(true, "Sucesso", paymentId, expectedQrCodeUrl));

            // Act
            var result = await _controller.GenerateQRCode(paymentId, model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GenerateQRCode_ReturnsBadRequestResult_WhenSuccessIsFalse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedResult = new GenerateQRCodeResultDto(false, "Failed to generate QR code", Guid.Empty, "");
            var model = new PaymentInputDto{ OrderId = Guid.NewGuid() };

            _mockPaymentAppService
                .Setup(service => service.GenerateQRCodeAsync(orderId, It.Is<PaymentInputDto>(x => x.OrderId == model.OrderId)))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GenerateQRCode(orderId, model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }
    }
}
