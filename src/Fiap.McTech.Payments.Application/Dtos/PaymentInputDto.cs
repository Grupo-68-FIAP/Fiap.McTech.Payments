namespace Fiap.McTech.Payments.Application.Dtos
{
    public class PaymentInputDto
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public ClientDto Client { get; set; } = new ClientDto();

        public class ClientDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Cpf { get; set; }
        }
    }
}
