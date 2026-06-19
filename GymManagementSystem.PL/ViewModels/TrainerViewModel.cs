using System;
using System.ComponentModel.DataAnnotations;
using GymManagementSystem.Domain;

namespace GymManagementSystem.PL.ViewModels;

public class TrainerViewModel
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

    [Required(ErrorMessage = "Specialty is required.")]
    [Display(Name = "Specialty")]
    public TrainerSpecialty Specialty { get; set; }

    [Required(ErrorMessage = "Hire Date is required.")]
    [Display(Name = "Hire Date")]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; } = DateTime.Today;
}
