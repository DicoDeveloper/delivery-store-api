using Application.Products.Commands.UpdateProduct;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;

namespace Application.Products.Extensions;

public static class UpdateProductCommandExtension
{
    public static void UpdateProductEntity(this UpdateProductCommand command, Product product)
    {
        product.Name = command.Product.Name;
        product.Price = command.Product.Price;
        product.Category = command.Product.Category!.StringToEnum<Category>();
        product.Description = command.Product.Description;
        product.StockQuantity = command.Product.StockQuantity;
    }
}