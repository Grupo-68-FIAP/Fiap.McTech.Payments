using Fiap.McTech.Payments.CrossCutting.Exceptions;
using Fiap.McTech.Payments.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Fiap.McTech.Payments.Infra.UnitTests.Context
{
    public class DataContextTests
    {
        private static DbContextOptions<DataContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void DataContext_ShouldInitializeWithInMemoryDatabase()
        {
            // Arrange
            var options = CreateInMemoryOptions();

            // Act
            using (var context = new DataContext(options))
            {
                // Assert
                Assert.NotNull(context);
                Assert.NotNull(context.Payments);
            }
        }

        [Fact]
        public void OnModelCreating_ShouldApplyMappingsCorrectly()
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
            }
        }

        [Fact]
        public void OnConfiguring_ShouldThrowExceptionIfConnectionStringIsMissing()
        {
            // Arrange
            Environment.SetEnvironmentVariable("CONNECTION_STRING", null);
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // Act & Assert
            var exception = Assert.Throws<DatabaseException>(() =>
            {
                using (var context = new DataContext(optionsBuilder.Options))
                {
                    context.Database.EnsureCreated();
                }
            });

            Assert.Equal("Environment variable [CONNECTION_STRING] is null.", exception.Message);
        }

        /*
         TODO: Este teste só roda no windows
         [Fact]
        public void OnConfiguring_ShouldUseConnectionStringFromEnvironmentVariable()
        {
            // Arrange
            //var connectionString = "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;";
            var connectionString = "DataSource=:memory:";
            Environment.SetEnvironmentVariable("CONNECTION_STRING", connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // Act
            using (var context = new DataContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                // Assert
                Assert.True(context.Database.IsRelational());
            }
        }*/
    }
}
