using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GymManagementSystem.PL.ViewModels;

public class MemberToUpdateViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = default!;

    [Display(Name = "Photo")]
    public string? Photo { get; set; }

    [Display(Name = "Photo")]
    public IFormFile? PhotoFile { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Phone Number is required.")]
    [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number must be a valid Egyptian number (010/011/012/015 followed by 8 digits).")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 digits.")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = default!;

    [Required(ErrorMessage = "Building Number is required.")]
    [Display(Name = "Building Number")]
    [StringLength(50, ErrorMessage = "Building Number cannot exceed 50 characters.")]
    public string BuildingNumber { get; set; } = default!;

    [Required(ErrorMessage = "Street is required.")]
    [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters.")]
    public string Street { get; set; } = default!;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string City { get; set; } = default!;
}
