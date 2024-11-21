using Fiap.McTech.Payments.Application.Dtos;
using Fiap.McTech.Payments.Application.Interfaces;
using Fiap.McTech.Payments.CrossCutting.Exceptions;
using Fiap.McTech.Payments.Presentation.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.McTech.Payments.BDD.StepDefinition
{
    [Binding]
    public class PaymentsStepDefinitions_Feature_02
    {
        private Guid _orderId;
        private IActionResult _response = default;
        private readonly string _invalidStatus = "invalid_status";

        private PaymentController _paymentController;
        private readonly Mock<IPaymentsAppService> _mockPaymentAppService;

        public PaymentsStepDefinitions_Feature_02()
        {
            _mockPaymentAppService = new Mock<IPaymentsAppService>();
            _paymentController = new PaymentController(_mockPaymentAppService.Object);
        }

        //Scenario 02
        [Given(@"que o status informado para o pagamento é inválido")]
        public void Givenqueostatusinformadoparaopagamentoinvlido()
        {
            _orderId = Guid.NewGuid();
        }

        [When(@"eu solicitar para atualizar o pagamento")]
        public async Task Wheneusolicitarparaatualizaropagamento()
        {
            _mockPaymentAppService.Setup(x =>
                x.UpdatePayment(
                    It.Is<Guid>(s => s == _orderId),
                    It.Is<string>(s => s == _invalidStatus)
                )
            )
            .ThrowsAsync(new EntityValidationException("Invalid Payment Status"));

            _response = await _paymentController.UpdatePayment(_orderId, _invalidStatus);
        }

        [Then(@"o sistema irá retornar erro")]
        public void Thenosistemairretornarerro()
        {
            // Verifica se a resposta é 200 OK
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(_response);

            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);

            // Extrai o corpo da resposta
            Assert.Equal("Invalid Payment Status", badRequestResult.Value);
        }
    }
}
