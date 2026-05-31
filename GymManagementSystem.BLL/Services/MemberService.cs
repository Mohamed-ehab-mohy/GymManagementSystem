using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.BLL.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(IMemberRepository memberRepository, IMembershipRepository membershipRepository, IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _membershipRepository = membershipRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return await _memberRepository.GetAllWithDetailsAsync();
    }

    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        return await _memberRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task AddMemberAsync(Member member)
    {
        await _memberRepository.AddAsync(member);
        await _unitOfWork.CompleteAsync();
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
        return (true, "Member deleted successfully.");
    }
}
