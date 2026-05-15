using System;
using System.Collections.Generic;

namespace GymManagementSystem.DAL.Entities;

public class Trainer : GymUser
{
    public string Specialization { get; set; } = null!;
    public DateTime? HireDate { get; set; }

    // Navigation Properties
    public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
}
