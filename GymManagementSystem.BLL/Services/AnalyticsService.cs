using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.DTOs;
using GymManagementSystem.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.BLL.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IClassSessionRepository _sessionRepository;
    private readonly IPlanRepository _planRepository;

    public AnalyticsService(
        IMemberRepository memberRepository,
        IBookingRepository bookingRepository,
        IMembershipRepository membershipRepository,
        IClassSessionRepository sessionRepository,
        IPlanRepository planRepository)
    {
        _memberRepository = memberRepository;
        _bookingRepository = bookingRepository;
        _membershipRepository = membershipRepository;
        _sessionRepository = sessionRepository;
        _planRepository = planRepository;
    }

    public async Task<AnalyticsDto> GetAnalyticsAsync(int months = 12)
    {
        var now = DateTime.UtcNow;
        var fromDate = now.AddMonths(-months);

        var newMembersPerMonth = await _memberRepository.Query()
            .Where(m => m.JoinDate >= fromDate && !m.IsDeleted)
            .GroupBy(m => new { m.JoinDate.Year, m.JoinDate.Month })
            .Select(g => new MonthlyCountDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Count = g.Count()
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToListAsync();

        var revenuePerMonth = await _membershipRepository.Query()
            .Include(m => m.Plan)
            .Where(m => m.StartDate >= fromDate && !m.IsDeleted)
            .GroupBy(m => new { m.StartDate.Year, m.StartDate.Month })
            .Select(g => new MonthlyRevenueDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Revenue = g.Sum(m => m.Plan.Price)
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToListAsync();

        var monthlyRevenue = await _membershipRepository.Query()
            .Include(m => m.Plan)
            .Where(m => m.StartDate.Year == now.Year && m.StartDate.Month == now.Month && !m.IsDeleted)
            .SumAsync(m => m.Plan.Price);

        var activeMembers = await _membershipRepository.Query()
            .Where(m => m.IsActive && !m.IsDeleted)
            .Select(m => m.MemberId)
            .Distinct()
            .CountAsync();

        var todaySessions = await _sessionRepository.Query()
            .CountAsync(s => s.ScheduleTime.Date == DateTime.Today && !s.IsDeleted);

        var expiringSoon = await _membershipRepository.Query()
            .CountAsync(m => m.IsActive && m.EndDate <= DateTime.Today.AddDays(7) && !m.IsDeleted);

        var topSessions = await _bookingRepository.Query()
            .Include(b => b.ClassSession)
            .Where(b => !b.IsDeleted)
            .GroupBy(b => new { b.ClassSessionId, b.ClassSession.Name, b.ClassSession.Capacity })
            .Select(g => new SessionStatDto
            {
                SessionName = g.Key.Name,
                Capacity = g.Key.Capacity,
                BookingCount = g.Count()
            })
            .OrderByDescending(s => s.BookingCount)
            .Take(5)
            .ToListAsync();

        var membersByPlan = await _membershipRepository.Query()
            .Include(m => m.Plan)
            .Where(m => m.IsActive && !m.IsDeleted)
            .GroupBy(m => m.Plan.Name)
            .Select(g => new PlanDistributionDto
            {
                PlanName = g.Key,
                MemberCount = g.Count()
            })
            .ToListAsync();

        return new AnalyticsDto
        {
            MonthlyRevenue = monthlyRevenue,
            ActiveMemberCount = activeMembers,
            TodaySessionsCount = todaySessions,
            ExpiringSoonCount = expiringSoon,
            NewMembersPerMonth = newMembersPerMonth,
            RevenuePerMonth = revenuePerMonth,
            TopSessions = topSessions,
            MembersByPlan = membersByPlan
        };
    }
}
