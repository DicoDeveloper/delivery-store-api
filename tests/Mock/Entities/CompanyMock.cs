using Domain.Entities;

namespace Tests.Mock.Entities;

public class CompanyMock
{
    private Company? _company;

    public CompanyMock Default()
    {
        _company = new()
        {
            Name = "Company test",
            Address = "Address test",
            Document = "11111111/1111-11",
            Local = "Local test",
            State = "State test",
            IsMainHeadquarter = true,
            ShippingCostOthersStates = 40,
            ShippingCostSameLocal = 10,
            ShippingCostSameState = 20
        };

        return this;
    }

    public CompanyMock WithIsMainHeadquarter(bool isMainHeadquarter)
    {
        _company!.IsMainHeadquarter = isMainHeadquarter;

        return this;
    }

    public CompanyMock WithShippingCostOthersStates(double shippingcost)
    {
        _company!.ShippingCostOthersStates = shippingcost;

        return this;
    }

    public CompanyMock WithShippingCostSameLocal(double shippingcost)
    {
        _company!.ShippingCostSameLocal = shippingcost;

        return this;
    }

    public CompanyMock WithShippingCostSameState(double shippingcost)
    {
        _company!.ShippingCostSameState = shippingcost;

        return this;
    }

    public Company? Build()
    {
        var company = _company;

        _company = null;

        return company;
    }
}