using Domain.Common;

namespace Domain.Entities;

public class Company : BaseEntity
{
    public required string Name { get; set; }
    public required string Document { get; set; }
    public bool IsMainHeadquarter { get; set; }
    public required string Address { get; set; }
    public required string Local { get; set; }
    public required string State { get; set; }
    public double? ShippingCostSameLocal { get; set; }
    public double? ShippingCostSameState { get; set; }
    public double? ShippingCostOthersStates { get; set; }
}