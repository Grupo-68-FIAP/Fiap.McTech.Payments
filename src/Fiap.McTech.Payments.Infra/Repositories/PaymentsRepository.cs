using Fiap.McTech.Payments.Domain.Interfaces.Repositories;
using Fiap.McTech.Payments.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Fiap.McTech.Payments.Infra.Repositories
{
    public class PaymentsRepository : RepositoryBase<Fiap.McTech.Payments.Domain.Entities.Payments>, IPaymentsRepository
    {
        public PaymentsRepository(DataContext context) : base(context) { }

        public async Task<Fiap.McTech.Payments.Domain.Entities.Payments> GetByOrderIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
