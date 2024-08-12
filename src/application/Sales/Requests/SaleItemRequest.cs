using System.ComponentModel.DataAnnotations;
using Application.Common.Validators;

namespace Application.Sales.Requests;

public class SaleItemRequest : RequestValidator
{
    [Required(ErrorMessage = "É preciso informar o Id do produto vinculado ao item da venda.")]
    public Guid ProductId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade de itens precisa ser maior que 0.")]
    public int Quantity { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor do item precisa ser maior que 0.")]
    public decimal UnitPrice { get; set; }
}