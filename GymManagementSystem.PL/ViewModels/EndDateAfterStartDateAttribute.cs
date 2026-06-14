using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.PL.ViewModels;

public class EndDateAfterStartDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime endDate)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty("StartDate");
            if (startDateProperty != null && startDateProperty.GetValue(validationContext.ObjectInstance) is DateTime startDate)
            {
                if (endDate <= startDate)
                {
                    return new ValidationResult(ErrorMessage ?? "End date must be after start date.");
                }
            }
        }
        return ValidationResult.Success;
    }
}
