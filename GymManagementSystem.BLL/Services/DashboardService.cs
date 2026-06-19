using GymManagementSystem.BLL.DTOs;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.BLL.Services;

public class DashboardService : IDashboardService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ITrainerRepository _trainerRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IClassSessionRepository _classSessionRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMembershipRepository _membershipRepository;

    public DashboardService(
        IMemberRepository memberRepository,
        ITrainerRepository trainerRepository,
        IPlanRepository planRepository,
        IClassSessionRepository classSessionRepository,
        IBookingRepository bookingRepository,
        IMembershipRepository membershipRepository)
    {
        _memberRepository = memberRepository;
        _trainerRepository = trainerRepository;
        _planRepository = planRepository;
        _classSessionRepository = classSessionRepository;
        _bookingRepository = bookingRepository;
        _membershipRepository = membershipRepository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var totalMembers = await _memberRepository.CountAsync();
        var totalTrainers = await _trainerRepository.CountAsync();
        var activePlans = await _planRepository.CountAsync(p => p.IsActive);
        var todaySessions = await _classSessionRepository.Query()
            .Where(cs => cs.ScheduleTime.Date == DateTime.Today && !cs.IsDeleted)
            .CountAsync();

        var todayAttendance = await _bookingRepository.Query()
            .CountAsync(b => b.ClassSession.ScheduleTime.Date == DateTime.Today && b.IsAttended && !b.IsDeleted);

        var expiringMemberships = await _membershipRepository.Query()
            .CountAsync(m => m.IsActive && m.EndDate <= DateTime.Today.AddDays(7) && !m.IsDeleted);

        return new DashboardStatsDto
        {
            TotalMembers = totalMembers,
            TotalTrainers = totalTrainers,
            ActivePlans = activePlans,
            TodaySessions = todaySessions,
            TodayAttendance = todayAttendance,
            ExpiringMemberships = expiringMemberships,
            TotalPlans = await _planRepository.CountAsync()
        };
    }
}
