using Fiap.McTech.Payments.Application.Dtos;
using Fiap.McTech.Payments.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Fiap.McTech.Payments.Presentation.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling operations related to payments.
    /// </summary>
    [Route("api/payment")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [ExcludeFromCodeCoverage]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentsAppService _paymentAppService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class with the specified payment application service.
        /// </summary>
        /// <param name="paymentAppService">The service to manage payment operations.</param>
        public PaymentController(IPaymentsAppService paymentAppService)
        {
            _paymentAppService = paymentAppService;
        }

        /// <summary>
        /// Generates a QR code for the specified order.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order for which to generate the QR code.</param>
        /// <returns>The URL of the generated QR code.</returns>
        /// <response code="200">Returns the URL of the generated QR code.</response>
        /// <response code="400">Indicates that there was an error generating the QR code.</response>
        [HttpPost("GenerateQRCode/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateQRCode(
            [FromRoute] Guid orderId,
            [FromBody] PaymentInputDto model
        )
        {
            var qrCodeUrl = await _paymentAppService.GenerateQRCodeAsync(orderId, model);
            if (!qrCodeUrl.Success)
                return BadRequest(new { message = qrCodeUrl.Message });

            return Ok(qrCodeUrl);
        }

        /// <summary>
        /// Processes a payment using the specified payment ID and Status
        /// </summary>
        /// <param name="paymentId">The unique identifier of the payment to be processed.</param>
        /// <param name="status">The status associated with the payment.</param>
        /// <returns>The result of the payment processing.</returns>
        /// <response code="200">Returns the result of the payment processing.</response>
        /// <response code="400">Indicates that there was an error processing the payment.</response>
        [HttpPost("{paymentId}/checkout")]
        [ProducesResponseType(typeof(PaymentOutputDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdatePayment([FromRoute] Guid paymentId, [FromBody] string status)
        {
            try
            {
                var paymentResult = await _paymentAppService.UpdatePayment(paymentId, status);
                if (!paymentResult.Success)
                    return BadRequest(paymentResult.Message);

                return Ok(paymentResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
