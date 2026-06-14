using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.PL.ViewModels;

public class CreateSessionViewModel
{
    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Start date and time is required")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start Date & Time")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(1);

    [Required(ErrorMessage = "End date and time is required")]
    [DataType(DataType.DateTime)]
    [Display(Name = "End Date & Time")]
    [EndDateAfterStartDate(ErrorMessage = "End date must be after start date.")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1).AddHours(1);

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 25, ErrorMessage = "Capacity must be between 1 and 25")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Trainer is required")]
    [Display(Name = "Trainer")]
    public int TrainerId { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem> TrainersList { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CategoriesList { get; set; } = new List<SelectListItem>();
}
