using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GymManagementSystem.PL.ViewModels;

public class MemberViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Phone Number is required.")]
    [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number must be a valid Egyptian number (010/011/012/015 followed by 8 digits).")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 digits.")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Date of Birth is required.")]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = new DateTime(2000, 1, 1);

    [Required(ErrorMessage = "Gender is required.")]
    [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
    public string Gender { get; set; } = null!;

    [Display(Name = "Photo")]
    public string? Photo { get; set; }

    [Display(Name = "Photo")]
    public IFormFile? PhotoFile { get; set; }

    [Display(Name = "Join Date")]
    [DataType(DataType.Date)]
    public DateTime JoinDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Street is required.")]
    [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters.")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "State is required.")]
    [StringLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "Zip Code is required.")]
    [StringLength(20, ErrorMessage = "Zip Code cannot exceed 20 characters.")]
    [Display(Name = "Zip Code")]
    public string ZipCode { get; set; } = null!;

    [Display(Name = "Emergency Contact Name")]
    public string? EmergencyContactName { get; set; }

    [Phone(ErrorMessage = "Invalid Emergency Contact Phone Number.")]
    [Display(Name = "Emergency Contact Phone")]
    public string? EmergencyContactPhone { get; set; }

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
    public string BloodType { get; set; } = null!;

    [Display(Name = "Health Notes")]
    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; set; }
}
