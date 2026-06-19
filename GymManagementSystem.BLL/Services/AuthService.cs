using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GymManagementSystem.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;

    public AuthService(IMemberRepository memberRepository, IUnitOfWork unitOfWork, IEmailService emailService, IMemoryCache cache)
    {
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<Result<ClaimsPrincipal>> LoginAsync(string email, string password)
    {
        var member = await _memberRepository.Query()
            .FirstOrDefaultAsync(m => m.Email == email && !m.IsDeleted);

        if (member == null || !BCrypt.Net.BCrypt.Verify(password, member.PasswordHash))
            return Result<ClaimsPrincipal>.Failure("Invalid email or password.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, member.Id.ToString()),
            new(ClaimTypes.Name, $"{member.FirstName} {member.LastName}"),
            new(ClaimTypes.Email, member.Email),
            new(ClaimTypes.Role, member.Role)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        return Result<ClaimsPrincipal>.Success(principal);
    }

    public async Task<Result> RegisterAsync(string firstName, string lastName, string email, string password, DateTime dateOfBirth, string gender)
    {
        var existing = await _memberRepository.Query()
            .AnyAsync(m => m.Email == email && !m.IsDeleted);

        if (existing)
            return Result.Failure("Email already registered.");

        var member = new Member
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            Role = "Member",
            DateOfBirth = dateOfBirth,
            Gender = gender,
            PhoneNumber = "",
            JoinDate = DateTime.Today,
            EmergencyContactName = "",
            EmergencyContactPhone = "",
            Address = new Address { Street = "", City = "", State = "", ZipCode = "" }
        };

        await _memberRepository.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var existing = await _memberRepository.Query()
            .AnyAsync(m => m.Email == email && !m.IsDeleted);

        if (existing)
            return Result.Failure("Email already registered.");

        var member = new Member
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            Role = "Member",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Gender = "Other",
            PhoneNumber = "",
            JoinDate = DateTime.Today,
            EmergencyContactName = "",
            EmergencyContactPhone = "",
            Address = new Address { Street = "", City = "", State = "", ZipCode = "" }
        };

        await _memberRepository.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<GymUser?> FindByEmailAsync(string email)
    {
        return await _memberRepository.Query()
            .FirstOrDefaultAsync(m => m.Email == email && !m.IsDeleted);
    }

    public async Task<ClaimsPrincipal> SignInUserAsync(GymUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        return new ClaimsPrincipal(identity);
    }

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        var user = await _memberRepository.Query()
            .FirstOrDefaultAsync(m => m.Email == email && !m.IsDeleted);

        if (user == null)
            return Result.Success();

        var otp = new Random().Next(100000, 999999).ToString();
        _cache.Set($"otp:{email}", otp, TimeSpan.FromMinutes(10));

        await _emailService.SendOtpAsync(email, otp);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string otp, string newPassword)
    {
        if (!_cache.TryGetValue($"otp:{email}", out string? cachedOtp) || cachedOtp != otp)
            return Result.Failure("Invalid or expired OTP.");

        var user = await _memberRepository.Query()
            .FirstOrDefaultAsync(m => m.Email == email && !m.IsDeleted);

        if (user == null)
            return Result.Failure("User not found.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
        _memberRepository.Update(user);
        await _unitOfWork.CompleteAsync();

        _cache.Remove($"otp:{email}");

        return Result.Success();
    }

    public async Task<Result> RegisterAdminAsync(string email, string password, string role)
    {
        var existing = await _memberRepository.Query()
            .AnyAsync(m => m.Email == email && !m.IsDeleted);

        if (existing)
            return Result.Failure("Email already registered.");

        var member = new Member
        {
            FirstName = role,
            LastName = "User",
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            Role = role,
            DateOfBirth = DateTime.Today.AddYears(-30),
            Gender = "Male",
            PhoneNumber = "",
            JoinDate = DateTime.Today,
            EmergencyContactName = "",
            EmergencyContactPhone = "",
            Address = new Address { Street = "", City = "", State = "", ZipCode = "" }
        };

        await _memberRepository.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }
}
