using Application.ViaCep.Dtos;

namespace Application.ViaCep.Services;

public interface IViaCepApiService
{
    Task<ViaCepInfoDto?> GetInfoAsync(string zipCode, CancellationToken cancellationToken = default);
}