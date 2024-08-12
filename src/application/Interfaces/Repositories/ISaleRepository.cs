using Domain.Entities;
using Domain.Filters;

namespace Application.Interfaces.Repositories;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync(SalesHistoryFilter? filter = null, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, bool withItems = false, CancellationToken cancellationToken = default);
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<Guid> ProcessSaleAsync(Sale sale, CancellationToken cancellationToken = default);
    Task CancelSaleAsync(Sale sale, CancellationToken cancellationToken = default);
}