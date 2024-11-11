using Fiap.McTech.Payments.Application.Dtos;
using Fiap.McTech.Payments.Domain.Interfaces.ExternalServices;

namespace Fiap.McTech.Payments.Application.Services.Mappers
{
    /// <summary>
    /// Classe contendo métodos de mapeamento para pagamentos.
    /// </summary>
    public static class PaymentMappers
    {
        /// <summary>
        /// Mapeia um objeto Order para um modelo de serviço PaymentRequest.
        /// </summary>
        /// <param name="order">O objeto Order a ser mapeado.</param>
        /// <returns>Um objeto PaymentRequest populado com os dados relevantes do objeto Order.</returns>
        public static PaymentRequest MapPaymentToServiceModel(this PaymentInputDto inputDto)
        {
            var request = new PaymentRequest()
            {
                TransactionAmount = inputDto.TotalAmount,
                Description = string.Empty,
                PaymentMethodId = "qrCode",
                Payer = new Payer
                {
                    Email = inputDto.Client.Email,
                    FirstName = inputDto.Client.Name ?? string.Empty,
                    LastName = inputDto.Client.Name ?? string.Empty,
                    Identification = new Identification
                    {
                        Type = "CPF",
                        Number = inputDto.Client.Cpf
                    }
                }
            };

            return request;
        }
    }
}
