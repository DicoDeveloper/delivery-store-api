using Application.Common.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CompanyRepository(IApplicationDbContext context) : ICompanyRepository
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Company?> GetMainCompanyAsync(CancellationToken cancellationToken = default)
        => await _context.Companies.SingleOrDefaultAsync(c => !c.IsDeleted && c.IsMainHeadquarter, cancellationToken);
}