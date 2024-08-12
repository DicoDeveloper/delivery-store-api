using System.ComponentModel.DataAnnotations;

namespace Application.Common.Validators;

public class RequestValidator
{
    [System.Text.Json.Serialization.JsonIgnore]
    public List<ValidationResult> ValidationResults { get; set; } = [];

    public string? GetErrors()
    {
        string errors = "";

        var errorList = ValidationResults.GroupBy(vr => vr.MemberNames.FirstOrDefault() ?? "Error")
                             .ToDictionary(g => g.Key, g => g.Select(vr => vr.ErrorMessage).ToArray());

        foreach (var error in errorList)
        {
            errors += $"{error.Value[0]} ";
        }

        return errors.ToString();
    }
}