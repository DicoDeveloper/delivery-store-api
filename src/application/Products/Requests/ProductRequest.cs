using System.ComponentModel.DataAnnotations;
using Application.Common.Validators;

namespace Application.Products.Requests;

public class ProductRequest : RequestValidator
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    public required string Name { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor precisa ser maior que 0.")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Categoria é obrigatória.")]
    public required string Category { get; set; }
    public string? Description { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Quantidade de estoque não pode ser menor que 0.")]
    public int StockQuantity { get; set; }
}