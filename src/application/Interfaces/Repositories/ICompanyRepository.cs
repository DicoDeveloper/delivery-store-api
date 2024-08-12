using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetMainCompanyAsync(CancellationToken cancellationToken = default);
}