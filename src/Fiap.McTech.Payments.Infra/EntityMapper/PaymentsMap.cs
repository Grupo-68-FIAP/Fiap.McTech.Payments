using Fiap.McTech.Payments.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.McTech.Payments.Infra.EntityMapper
{
    internal class PaymentsMap : IEntityTypeConfiguration<Fiap.McTech.Payments.Domain.Entities.Payments>
    {
        public void Configure(EntityTypeBuilder<Fiap.McTech.Payments.Domain.Entities.Payments> builder)
        {
            builder.ToTable(nameof(DataContext.Payments));

            builder.HasKey(c => c.Id);

            builder.Property(c => c.OrderId)
                .HasConversion<Guid>()
                .IsRequired();

            builder.Property(c => c.ClientId)
                .HasConversion<Guid>()
                .IsRequired(false);

            builder.Property(c => c.Value)
                .HasPrecision(14, 2)
                .IsRequired();

            builder.Property(c => c.Discount)
                .HasPrecision(14, 2)
                .IsRequired();

            builder.Property(c => c.AdditionalFees)
                .HasPrecision(14, 2)
                .IsRequired();

            builder.Property(c => c.Method)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.Notes)
                .IsRequired();
        }
    }
}
