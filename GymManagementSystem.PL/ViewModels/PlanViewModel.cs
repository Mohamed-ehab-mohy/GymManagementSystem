using System;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.PL.ViewModels;

public class PlanViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The plan name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "The plan description is required.")]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Duration is required.")]
    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
    [Display(Name = "Duration (Days)")]
    public int DurationDays { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, 100000, ErrorMessage = "Price must be a positive number.")]
    [Display(Name = "Price (EGP)")]
    public decimal Price { get; set; }

    [Display(Name = "Is Active?")]
    public bool IsActive { get; set; } = true;
}
