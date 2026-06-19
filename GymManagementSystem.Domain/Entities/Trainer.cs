using System;
using System.Collections.Generic;

namespace GymManagementSystem.Domain;

public class Trainer : GymUser
{
    public TrainerSpecialty Specialty { get; set; }
    public DateTime? HireDate { get; set; }

    public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
}
