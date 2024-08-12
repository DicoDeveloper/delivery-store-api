using Moq;
using Moq.Protected;
using System.Net;
using Application.ViaCep.Services;
using Infrastructure.Services;

namespace Tests.Infrastructure.Services;

public class ViaCepApiServiceTest
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly IViaCepApiService _viaCepApiService;

    public ViaCepApiServiceTest()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new("https://viacep.com.br/")
        };

        _viaCepApiService = new ViaCepApiService(_httpClient);
    }

    [Fact]
    public async Task ShouldReturnViaCepInfoDto()
    {
        var jsonResponse = "{\"cep\": \"20031-110\",\"logradouro\": \"Rua Manuel de Carvalho\",\"complemento\": \"\",\"unidade\": \"\",\"bairro\": \"Centro\",\"localidade\": \"Rio de Janeiro\",\"uf\": \"RJ\",\"ibge\": \"3304557\",\"gia\": \"\",\"ddd\": \"21\",\"siafi\": \"6001\"}";

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var result = await _viaCepApiService.GetInfoAsync("20031110");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenApiCallFails()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var result = await _viaCepApiService.GetInfoAsync("20031110");

        Assert.Null(result);
    }
}