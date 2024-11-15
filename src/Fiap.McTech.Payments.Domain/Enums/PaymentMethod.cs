using System.ComponentModel;

namespace Fiap.McTech.Payments.Domain.Enums
{
    /// <summary>
    /// Represents the method used for payment.
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// No payment method.
        /// </summary>
        [Description("Nenhum")]
        None = -1,

        /// <summary>
        /// Payment made via credit card.
        /// </summary>
        [Description("Cartão de Crédito")]
        CreditCard,

        /// <summary>
        /// Payment made via debit card.
        /// </summary>
        [Description("Cartão de Débito")]
        DebitCard,

        /// <summary>
        /// Payment made via Pix.
        /// </summary>
        [Description("PIX")]
        Pix,

        /// <summary>
        /// Payment made via QR code.
        /// </summary>
        [Description("QR-Code")]
        QrCode
    }
}
