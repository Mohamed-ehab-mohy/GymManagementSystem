using System.Linq.Expressions;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.DTOs;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GymManagementSystem.BLL.Services;

    public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public MemberService(IMemberRepository memberRepository, IMembershipRepository membershipRepository, IBookingRepository bookingRepository, IUnitOfWork unitOfWork, ILogger<MemberService>? logger = null)
    {
        _memberRepository = memberRepository;
        _membershipRepository = membershipRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger ?? NullLogger<MemberService>.Instance;
    }

    public async Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return await _memberRepository.GetAllWithDetailsAsync();
    }

    public async Task<PagedResult<Member>> GetPagedMembersAsync(int page, int pageSize, string? search = null, string? sortBy = null, bool ascending = true)
    {
        Expression<Func<Member, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            filter = m => m.FirstName.Contains(search) || m.LastName.Contains(search) || m.Email!.Contains(search) || m.PhoneNumber!.Contains(search);
        }

        return await _memberRepository.GetPagedAsync(page, pageSize, filter, sortBy, ascending);
    }

    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        return await _memberRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task AddMemberAsync(Member member)
    {
        await _memberRepository.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Member created: member {MemberId} ({Email})", member.Id, member.Email);
    }

    public async Task UpdateMemberAsync(Member member)
    {
        _memberRepository.Update(member);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<(bool Success, string Message)> DeleteMemberAsync(int id)
    {
        var member = await _memberRepository.GetByIdWithDetailsAsync(id);
        if (member == null)
            return (false, "Member not found.");

        var activeMemberships = await _membershipRepository.GetActiveByMemberIdAsync(id);
        if (activeMemberships.Any())
            return (false, "Cannot delete member. Active memberships found.");

        var bookings = await _bookingRepository.GetByMemberIdAsync(id);
        if (bookings.Any())
            return (false, "Cannot delete member. Active bookings found.");

        _memberRepository.Delete(member);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Member deleted: member {MemberId} ({Email})", id, member.Email);
        return (true, "Member deleted successfully.");
    }

    public async Task<MemberDashboardDto?> GetMemberDashboardAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdWithDetailsAsync(memberId);
        if (member == null) return null;

        var activeMembership = await _membershipRepository.Query()
            .Include(m => m.Plan)
            .Where(m => m.MemberId == memberId && m.IsActive && !m.IsDeleted)
            .FirstOrDefaultAsync();

        var upcomingBookings = await _bookingRepository.Query()
            .Include(b => b.ClassSession).ThenInclude(s => s!.Trainer)
            .Include(b => b.ClassSession).ThenInclude(s => s!.Category)
            .Where(b => b.MemberId == memberId && !b.IsDeleted && b.ClassSession.ScheduleTime > DateTime.Now)
            .OrderBy(b => b.ClassSession.ScheduleTime)
            .Take(5)
            .ToListAsync();

        var totalBooked = await _bookingRepository.Query()
            .CountAsync(b => b.MemberId == memberId && !b.IsDeleted);

        var totalAttended = await _bookingRepository.Query()
            .CountAsync(b => b.MemberId == memberId && b.IsAttended && !b.IsDeleted);

        decimal? bmi = null;
        string? bmiCategory = null;
        if (member.HealthRecord?.Height > 0 && member.HealthRecord?.Weight > 0)
        {
            var heightInMeters = member.HealthRecord.Height / 100m;
            bmi = Math.Round(member.HealthRecord.Weight / (heightInMeters * heightInMeters), 1);
            bmiCategory = bmi switch
            {
                < 18.5m => "Underweight",
                < 25m => "Normal",
                < 30m => "Overweight",
                _ => "Obese"
            };
        }

        var daysRemaining = activeMembership != null
            ? (activeMembership.EndDate - DateTime.Today).Days
            : 0;

        return new MemberDashboardDto
        {
            MemberName = $"{member.FirstName} {member.LastName}",
            Photo = member.Photo,
            JoinDate = member.JoinDate,
            HasActiveMembership = activeMembership != null,
            PlanName = activeMembership?.Plan?.Name,
            MembershipStart = activeMembership?.StartDate,
            MembershipEnd = activeMembership?.EndDate,
            DaysRemaining = Math.Max(0, daysRemaining),
            Weight = member.HealthRecord?.Weight,
            Height = member.HealthRecord?.Height,
            Bmi = bmi,
            BmiCategory = bmiCategory,
            UpcomingSessions = upcomingBookings.Select(b => new UpcomingSessionDto
            {
                SessionName = b.ClassSession.Name,
                ScheduleTime = b.ClassSession.ScheduleTime,
                TrainerName = $"{b.ClassSession.Trainer.FirstName} {b.ClassSession.Trainer.LastName}",
                CategoryName = b.ClassSession.Category.CategoryName
            }).ToList(),
            TotalBooked = totalBooked,
            TotalAttended = totalAttended,
            AttendanceRate = totalBooked > 0 ? Math.Round((double)totalAttended / totalBooked * 100, 1) : 0
        };
    }
}
