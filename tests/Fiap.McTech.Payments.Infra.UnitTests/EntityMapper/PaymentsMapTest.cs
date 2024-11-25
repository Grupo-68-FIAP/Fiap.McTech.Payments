using Fiap.McTech.Payments.Infra.Context;
using Fiap.McTech.Payments.Infra.EntityMapper;
using Microsoft.EntityFrameworkCore;

namespace Fiap.McTech.Payments.Infra.UnitTests.EntityMapper
{
    public class PaymentsMapTests
    {
        private static DbContextOptions<DataContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void PaymentsMap_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var options = CreateInMemoryOptions();

            using (var context = new DataContext(options))
            {
                // Act
                var model = context.Model.FindEntityType(typeof(Domain.Entities.Payments));

                // Assert
                Assert.NotNull(model);
                Assert.Equal("Payments", model.GetTableName());
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.Id));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.OrderId));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.ClientId));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.Value));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.Discount));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.AdditionalFees));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.Method));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.Status));
                Assert.Contains(model.GetProperties(), p => p.Name == nameof(Domain.Entities.Payments.Notes));
            }
        }

        [Fact]
        public void PaymentsMap_ShouldHaveCorrectPrecisionForDecimalProperties()
        {
            // Arrange
            var options = CreateInMemoryOptions();

            using (var context = new DataContext(options))
            {
                // Act
                var model = context.Model.FindEntityType(typeof(Domain.Entities.Payments));

                // Assert
                var valueProperty = model.FindProperty(nameof(Domain.Entities.Payments.Value));
                var discountProperty = model.FindProperty(nameof(Domain.Entities.Payments.Discount));
                var additionalFeesProperty = model.FindProperty(nameof(Domain.Entities.Payments.AdditionalFees));

                Assert.Equal(14, valueProperty.GetPrecision());
                Assert.Equal(2, valueProperty.GetScale());

                Assert.Equal(14, discountProperty.GetPrecision());
                Assert.Equal(2, discountProperty.GetScale());

                Assert.Equal(14, additionalFeesProperty.GetPrecision());
                Assert.Equal(2, additionalFeesProperty.GetScale());
            }
        }

        [Fact]
        public void PaymentsMap_ShouldHaveRequiredAndOptionalFieldsCorrectlyConfigured()
        {
            // Arrange
            var options = CreateInMemoryOptions();

            using (var context = new DataContext(options))
            {
                // Act
                var model = context.Model.FindEntityType(typeof(Domain.Entities.Payments));

                // Assert
                Assert.False(model.FindProperty(nameof(Domain.Entities.Payments.OrderId)).IsColumnNullable());
                Assert.True(model.FindProperty(nameof(Domain.Entities.Payments.ClientId)).IsColumnNullable());
                Assert.False(model.FindProperty(nameof(Domain.Entities.Payments.Value)).IsColumnNullable());
                Assert.False(model.FindProperty(nameof(Domain.Entities.Payments.Discount)).IsColumnNullable());
                Assert.False(model.FindProperty(nameof(Domain.Entities.Payments.AdditionalFees)).IsColumnNullable());
                Assert.False(model.FindProperty(nameof(Domain.Entities.Payments.Method)).IsColumnNullable());
                Assert.False(model.FindProperty(nameof(Domain.Entities.Payments.Status)).IsColumnNullable());
                Assert.False(model.FindProperty(nameof(Domain.Entities.Payments.Notes)).IsColumnNullable());
            }
        }
    }
}
