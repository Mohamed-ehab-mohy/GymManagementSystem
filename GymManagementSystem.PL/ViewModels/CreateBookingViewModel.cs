using System.Collections.Generic;
using GymManagementSystem.Domain;

namespace GymManagementSystem.PL.ViewModels;

public class CreateBookingViewModel
{
    public int MemberId { get; set; }
    public int ClassSessionId { get; set; }
    public IEnumerable<Member> Members { get; set; } = [];
    public IEnumerable<ClassSession> Sessions { get; set; } = [];
}
