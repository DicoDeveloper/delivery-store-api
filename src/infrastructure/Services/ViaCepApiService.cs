using Application.ViaCep.Dtos;
using Application.ViaCep.Services;
using Newtonsoft.Json;

namespace Infrastructure.Services;

public class ViaCepApiService : IViaCepApiService
{
    private readonly HttpClient _httpClient;

    public ViaCepApiService(HttpClient httpClient)
        => _httpClient = httpClient;

    public async Task<ViaCepInfoDto?> GetInfoAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        var url = $"ws/{zipCode}/json/";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonConvert.DeserializeObject<ViaCepInfoDto?>(content);
    }
}