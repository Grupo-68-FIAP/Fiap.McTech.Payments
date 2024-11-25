using Fiap.McTech.Payments.Domain.Enums;
using Fiap.McTech.Payments.Infra.Context;
using Fiap.McTech.Payments.Infra.Repositories;

namespace Fiap.McTech.Payments.Infra.UnitTests.Repositories
{
    public class PaymentsRepositoryTest : RepositoryBaseUnitTests<Domain.Entities.Payments>
    {
        protected override PaymentsRepository GetRepository(DataContext context)
        {
            return new PaymentsRepository(context);
        }

        protected override Domain.Entities.Payments MakeNewEntity()
        {
            var i = new Random().Next(1000, 9999);
            return new Domain.Entities.Payments(
                clientId: Guid.NewGuid(),
                orderId: Guid.NewGuid(),
                value: 10,
                clientName: $"Client{i}",
                clientEmail: $"client_{i}@teste.com",
                method: PaymentMethod.QrCode,
                status: PaymentStatus.Completed
                );
        }

        [Fact]
        public async Task GetByOrderIdAsync_ShouldReturnPayment_WhenPaymentIsFound()
        {
            Before();
            if (_context == null)
                Assert.Fail();

            // Arrange
            var repository = GetRepository(_context);

            var payment = MakeNewEntity();
            var dbSet = _context.Set<Domain.Entities.Payments>();
            await dbSet.AddAsync(payment);
            await _context.SaveChangesAsync();

            // Act
            var result = await repository.GetByOrderIdAsync(payment.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payment, result);

            After();
        }
    }
}
