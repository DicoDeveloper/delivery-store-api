using System.ComponentModel.DataAnnotations;
using Application.Common.Validators;

namespace Application.Common.Extensions;

public static class RequestValidatorExtension
{
    public static bool IsValid(this RequestValidator request)
    {
        var validationContext = new ValidationContext(request, serviceProvider: null, items: null);

        return Validator.TryValidateObject(request, validationContext, request.ValidationResults, true);
    }
}