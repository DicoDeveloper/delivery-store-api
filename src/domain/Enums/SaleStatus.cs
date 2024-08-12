using System.ComponentModel;

namespace Domain.Enums;

public enum SaleStatus
{
    [Description("")]
    Undefined = 0,
    [Description("Criada")]
    Created = 1,
    [Description("Paga")]
    Paid = 2,
    [Description("Entregue")]
    Delivered = 3,
    [Description("Cancelada")]
    Canceled = 4
}