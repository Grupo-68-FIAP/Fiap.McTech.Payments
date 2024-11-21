using Fiap.McTech.Payments.Application.Dtos;
using Fiap.McTech.Payments.Application.Interfaces;
using Fiap.McTech.Payments.CrossCutting.Exceptions;
using Fiap.McTech.Payments.Presentation.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.McTech.Payments.BDD.StepDefinition
{
    [Binding]
    public class PaymentsStepDefinitions_Feature_01
    {
        private PaymentRequestInput _modelInput = default;
        private Guid _orderId;
        private IActionResult _response = default;

        private PaymentController _paymentController;
        private readonly Mock<IPaymentsAppService> _mockPaymentAppService;

        public PaymentsStepDefinitions_Feature_01()
        {
            _mockPaymentAppService = new Mock<IPaymentsAppService>();
            _paymentController = new PaymentController(_mockPaymentAppService.Object);
        }

        [Given(@"que o valor de OrderId é um UUID e que os dados do objeto da requisição sejam ficticios")]
        public void GivenqueovalordeOrderIdumUUIDequeosdadosdoobjetodarequisiosejamficticios()
        {
            _orderId = Guid.NewGuid();

            _modelInput = new PaymentRequestInput
            {
                OrderId = _orderId,
                Model = new PaymentInputDto
                {
                    OrderId = _orderId,
                    TotalAmount = 100,
                    Client = new PaymentInputDto.ClientDto
                    {
                        Cpf = "12345678999",
                        Email = "teste@teste.com",
                        Id = Guid.NewGuid(),
                        Name = "Cliente Teste BDD"
                    }
                }
            };
        }

        [When(@"eu solicitar para o método GenerateQRCode e passar os parâmetros")]
        public async Task WheneusolicitarparaomtodoGenerateQRCodeepassarosparmetros()
        {
            _mockPaymentAppService.Setup(x =>
                x.GenerateQRCodeAsync(
                    It.Is<Guid>(s => s == _orderId),
                    It.Is<PaymentInputDto>(s => s.OrderId == _orderId)
                )
            )
            .ReturnsAsync(new GenerateQRCodeResultDto
            (
                message: "OK",
                paymentId: Guid.NewGuid(),
                qrCode: "xxx",
                success: true
            ));

            _response = await _paymentController.GenerateQRCode(_orderId, _modelInput.Model);
        }

        [Then(@"o status de resposta deve ser (.*) OK")]
        public void ThenostatusderespostadeveserOK(int args1)
        {
            // Verifica se a resposta é 200 OK
            var okResult = Assert.IsType<OkObjectResult>(_response);

            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);

            // Extrai o corpo da resposta
            var responseBody = okResult.Value as GenerateQRCodeResultDto;
            Assert.NotNull(responseBody?.QRCode);
        }

        //
        //
        // INTERNAL CLASS AND METHODS
        //
        //

        private class PaymentRequestInput
        {
            public Guid OrderId { get; set; }
            public PaymentInputDto Model { get; set; }
        }
    }
}
