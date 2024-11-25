namespace Fiap.McTech.Payments.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Represents a repository interface for CRUD operations with payments in the McTech domain.
    /// </summary>
    public interface IPaymentsRepository : IRepositoryBase<Fiap.McTech.Payments.Domain.Entities.Payments>
    {
        /// <summary>
        /// Asynchronously retrieves a payment by its associated order identifier.
        /// </summary>
        /// <param name="orderId">The identifier of the order associated with the payment.</param>
        /// <returns>A task representing the asynchronous operation, containing the payment associated with the specified order identifier, if found; otherwise, null.</returns>
        Task<Fiap.McTech.Payments.Domain.Entities.Payments> GetByOrderIdAsync(Guid orderId);
    }
}
