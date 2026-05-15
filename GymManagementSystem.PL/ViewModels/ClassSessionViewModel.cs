using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.PL.ViewModels;

public class ClassSessionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Class Session Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Start Time is required.")]
    [Display(Name = "Start Time")]
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; } = DateTime.Now.AddDays(1);

    [Required(ErrorMessage = "End Time is required.")]
    [Display(Name = "End Time")]
    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; } = DateTime.Now.AddDays(1).AddHours(1);

    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Trainer is required.")]
    [Display(Name = "Trainer")]
    public int TrainerId { get; set; }

    // For Display
    public string? TrainerName { get; set; }

    // For Dropdown selection in Create/Edit
    public IEnumerable<SelectListItem>? TrainersList { get; set; }
}
