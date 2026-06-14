using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.PL.ViewModels;

public class HealthRecordViewModel
{
    [Required(ErrorMessage = "Height is required.")]
    [Range(50, 300, ErrorMessage = "Height must be between 50 and 300 cm.")]
    [Display(Name = "Height (cm)")]
    public decimal Height { get; set; }

    [Required(ErrorMessage = "Weight is required.")]
    [Range(10, 500, ErrorMessage = "Weight must be between 10 and 500 kg.")]
    [Display(Name = "Weight (kg)")]
    public decimal Weight { get; set; }

    [Required(ErrorMessage = "Blood Type is required.")]
    [StringLength(5, ErrorMessage = "Blood Type cannot exceed 5 characters.")]
    [Display(Name = "Blood Type")]
    public string BloodType { get; set; } = default!;

    [Display(Name = "Health Notes")]
    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; set; }
}
