using System.ComponentModel.DataAnnotations;
using Application.Common.Validators;

namespace Application.Sales.Requests;

public class SaleRequest : RequestValidator
{
    [Required(ErrorMessage = "Data da venda é obrigatória.")]
    public DateTime? SaleDate { get; set; }
    [Required(ErrorMessage = "É preciso informar pelo menos um item.")]
    public List<SaleItemRequest>? Items { get; set; } = [];
}