using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GymManagementSystem.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IRepository<PasswordResetToken> _passwordResetTokenRepo;
    private readonly ILogger _logger;

    public AuthService(IMemberRepository memberRepository, IUnitOfWork unitOfWork, IEmailService emailService, IRepository<PasswordResetToken> passwordResetTokenRepo, ILogger<AuthService>? logger = null)
    {
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _passwordResetTokenRepo = passwordResetTokenRepo;
        _logger = logger ?? NullLogger<AuthService>.Instance;
    }

    public async Task<Result<ClaimsPrincipal>> LoginAsync(string email, string password)
    {
        var member = await _memberRepository.Query()
            .FirstOrDefaultAsync(m => m.Email == email && !m.IsDeleted);

        if (member == null || !BCrypt.Net.BCrypt.Verify(password, member.PasswordHash))
        {
            _logger.LogWarning("Login failed for email {Email}", email);
            return Result<ClaimsPrincipal>.Failure("Invalid email or password.");
        }

        _logger.LogInformation("Login succeeded for member {MemberId} ({Email})", member.Id, email);

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
        {
            _logger.LogWarning("Registration failed: email {Email} already exists", email);
            return Result.Failure("Email already registered.");
        }

        var member = BuildMember(firstName, lastName, email, password, "Member", dateOfBirth, gender);

        await _memberRepository.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Member registered: member {MemberId} ({Email})", member.Id, email);
        return Result.Success();
    }

    public async Task<Result> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        return await RegisterAsync(firstName, lastName, email, password, DateTime.Today.AddYears(-20), "Other");
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
        var codeHash = BCrypt.Net.BCrypt.HashPassword(otp);

        var existingTokens = await _passwordResetTokenRepo.Query()
            .Where(t => t.UserId == user.Id && !t.IsUsed)
            .ToListAsync();

        foreach (var t in existingTokens)
        {
            t.IsUsed = true;
        }

        var token = new PasswordResetToken
        {
            UserId = user.Id,
            CodeHash = codeHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };

        await _passwordResetTokenRepo.AddAsync(token);
        await _unitOfWork.CompleteAsync();

        await _emailService.SendOtpAsync(email, otp);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string otp, string newPassword)
    {
        var user = await _memberRepository.Query()
            .FirstOrDefaultAsync(m => m.Email == email && !m.IsDeleted);

        if (user == null)
            return Result.Failure("User not found.");

        var token = await _passwordResetTokenRepo.Query()
            .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        if (token == null || !BCrypt.Net.BCrypt.Verify(otp, token.CodeHash))
            return Result.Failure("Invalid or expired OTP.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
        token.IsUsed = true;
        _memberRepository.Update(user);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result> RegisterAdminAsync(string email, string password, string role)
    {
        var existing = await _memberRepository.Query()
            .AnyAsync(m => m.Email == email && !m.IsDeleted);

        if (existing)
            return Result.Failure("Email already registered.");

        var member = BuildMember(role, "User", email, password, role, DateTime.Today.AddYears(-30), "Male");

        await _memberRepository.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    private static string GenerateStablePhone(string email)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(email));
        var suffix = BitConverter.ToUInt32(hash, 0) % 100000000;
        return $"010{suffix:D8}";
    }

    private static Member BuildMember(string firstName, string lastName, string email, string password, string role, DateTime dateOfBirth, string gender)
    {
        return new Member
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            Role = role,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            PhoneNumber = GenerateStablePhone(email),
            JoinDate = DateTime.Today,
            EmergencyContactName = "",
            EmergencyContactPhone = "",
            Address = new Address { Street = "", City = "", State = "", ZipCode = "" }
        };
    }
}
