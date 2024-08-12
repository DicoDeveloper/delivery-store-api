using System.ComponentModel;

namespace Domain.Enums;

public enum SaleItemStatus
{
    [Description("")]
    Undefined = 0,
    [Description("Ativo")]
    Active = 1,
    [Description("Cancelado")]
    Canceled = 2
}