using Fiap.McTech.Payments.Application.Dtos;

namespace Fiap.McTech.Payments.Application.Interfaces
{
    /// <summary>
    /// Interface for Payment Application Service.
    /// </summary>
    public interface IPaymentsAppService
    {
        /// <summary>
        /// Generates a QR code asynchronously for the specified order ID.
        /// </summary>
        /// <param name="orderId">The ID of the order for which to generate the QR code.</param>
        /// <param name="model">Model with all data order for which to generate the QR code.</param>
        /// <returns>A task representing the asynchronous operation that returns the result of generating the QR code.</returns>
        Task<GenerateQRCodeResultDto> GenerateQRCodeAsync(Guid orderId, PaymentInputDto model);

        /// <summary>
        /// Processes a payment asynchronously using the provided payment ID and status by web hook.
        /// </summary>
        /// <param name="paymentId">The ID of the payment to process.</param>
        /// <param name="status">The status for payment.</param>
        /// <returns>A task representing the asynchronous operation that returns the payment output DTO.</returns>
        Task<PaymentOutputDto> UpdatePayment(Guid paymentId, string status);
    }
}
