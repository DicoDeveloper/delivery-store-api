using System.ComponentModel;

namespace Domain.Enums;

public enum Category
{
    [Description("")]
    Undefined = 0,
    [Description("Eletrônicos")]
    Electronics = 1,
    [Description("Roupas")]
    Clothing = 2,
    [Description("Eletrodomésticos")]
    HomeAppliances = 3,
    [Description("Livros")]
    Books = 4,
    [Description("Brinquedos")]
    Toys = 5,
    [Description("Mantimentos")]
    Groceries = 6,
    [Description("Esportivos")]
    Sports = 7,
    [Description("Beleza")]
    Beauty = 8,
    [Description("Móveis")]
    Furniture = 9,
    [Description("Desconhecido")]
    Unknown = 99
}