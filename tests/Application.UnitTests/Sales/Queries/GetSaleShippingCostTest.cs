using Application.Common.Extensions;
using Application.Interfaces.Repositories;
using Application.Sales.Queries.GetSaleShippingCost;
using Application.ViaCep.Dtos;
using Application.ViaCep.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Entities;

namespace Tests.Application.UnitTests.Sales.Queries;

public class GetSaleShippingCostTest
{
    private readonly Mock<ICompanyRepository> _companyRepository;
    private readonly Mock<IViaCepApiService> _viaCepApiService;
    private readonly Mock<ILogger<GetSaleShippingCostQueryHandler>> _logger;
    private readonly GetSaleShippingCostQueryHandler _getSaleShippingCostCommandHandler;

    public GetSaleShippingCostTest()
    {
        _companyRepository = new Mock<ICompanyRepository>();
        _viaCepApiService = new Mock<IViaCepApiService>();
        _logger = new Mock<ILogger<GetSaleShippingCostQueryHandler>>();
        _getSaleShippingCostCommandHandler = new(_companyRepository.Object, _viaCepApiService.Object, _logger.Object);
    }

    [Theory]
    [InlineData("Test", "Test", 100)]
    [InlineData("Local test", "Test", 50)]
    [InlineData("Test", "State test", 80)]
    public async void ShouldReturnShippingCostOthersStatesOrShippingCostSameLocalOrShippingCostSameState(string localiadde, string uf, double shippingCost)
    {
        // arrange
        var query = new GetSaleShippingCostQuery { ZipCode = "00000000" };

        var mainCompany = new CompanyMock().Default()
                                           .WithShippingCostOthersStates(100)
                                           .WithShippingCostSameLocal(50)
                                           .WithShippingCostSameState(80)
                                           .Build();

        _companyRepository.Setup(p =>
            p.GetMainCompanyAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(mainCompany);

        var viaCepInfo = new ViaCepInfoDto { Localidade = localiadde, Uf = uf, Erro = false };

        _viaCepApiService.Setup(p =>
            p.GetInfoAsync(query.ZipCode.ClearToCompare(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(viaCepInfo);

        // act
        var result = await _getSaleShippingCostCommandHandler.Handle(query, It.IsAny<CancellationToken>());

        // assert
        Assert.Equivalent(result, shippingCost);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("      ")]
    [InlineData("000")]
    [InlineData("0000000")]
    [InlineData("0000000000000")]
    public async Task ShouldThrowException_WhenZipCodeIsNullOrWhiteSpaceOrHasInvalidFormat(string invalidZipCode)
    {
        // arrange
        var query = new GetSaleShippingCostQuery { ZipCode = invalidZipCode };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _getSaleShippingCostCommandHandler.Handle(query, It.IsAny<CancellationToken>()));
    }

    public static TheoryData<ViaCepInfoDto?> InvalidViaCepInfo
       => new()
       {
            { null },
            { new() },
            { new() { Localidade = "Localidade test", Erro = false } },
            { new() { Uf = "Uf test", Erro = false } },
            { new() { Localidade = "Localidade test", Uf = "Uf test", Erro = true } },
       };

    [Theory]
    [MemberData(nameof(InvalidViaCepInfo))]
    public async Task ShouldThrowException_WhenViaCepInfoIsInvalid(ViaCepInfoDto? invalidViaCepInfoDto)
    {
        // arrange
        var query = new GetSaleShippingCostQuery { ZipCode = "00000000" };

        var mainCompany = new CompanyMock().Default().Build();

        _companyRepository.Setup(p =>
            p.GetMainCompanyAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(mainCompany);

        _viaCepApiService.Setup(p =>
            p.GetInfoAsync(query.ZipCode.ClearToCompare(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(invalidViaCepInfoDto);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _getSaleShippingCostCommandHandler.Handle(query, It.IsAny<CancellationToken>()));
    }
}