using System.Security.Claims;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.Tests.Infrastructure;
using GymManagementSystem.Domain;
using NSubstitute;
using Shouldly;

namespace GymManagementSystem.BLL.Tests;

public class AuthServiceTests
{
    private readonly IMemberRepository _memberRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IRepository<PasswordResetToken> _tokenRepo;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _memberRepo = Substitute.For<IMemberRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _emailService = Substitute.For<IEmailService>();
        _tokenRepo = Substitute.For<IRepository<PasswordResetToken>>();
        _authService = new AuthService(_memberRepo, _unitOfWork, _emailService, _tokenRepo);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsFailure()
    {
        var email = "test@test.com";
        _memberRepo.Query().Returns(new TestAsyncEnumerable<Member>([
            new Member { Email = email }
        ]).AsQueryable());

        var result = await _authService.RegisterAsync("Test", "User", email, "Password123", DateTime.Today.AddYears(-20), "Male");

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldContain("email", Case.Insensitive);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        var email = "test@test.com";
        var member = new Member
        {
            Id = 1,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPass", 12),
            FirstName = "Test",
            LastName = "User",
            Role = "Member"
        };
        _memberRepo.Query().Returns(new TestAsyncEnumerable<Member>([member]).AsQueryable());

        var result = await _authService.LoginAsync(email, "wrongPass");

        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task LoginAsync_CorrectPassword_ReturnsSuccessWithRoleClaim()
    {
        var email = "test@test.com";
        var member = new Member
        {
            Id = 1,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPass", 12),
            FirstName = "Test",
            LastName = "User",
            Role = "Member"
        };
        _memberRepo.Query().Returns(new TestAsyncEnumerable<Member>([member]).AsQueryable());

        var result = await _authService.LoginAsync(email, "correctPass");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.FindFirst(ClaimTypes.Role)?.Value.ShouldBe("Member");
        result.Value.FindFirst(ClaimTypes.NameIdentifier)?.Value.ShouldBe("1");
        result.Value.FindFirst(ClaimTypes.Email)?.Value.ShouldBe(email);
    }

    [Fact]
    public async Task RegisterAsync_NewEmail_ReturnsSuccess()
    {
        var email = "new@test.com";
        _memberRepo.Query().Returns(new TestAsyncEnumerable<Member>([]).AsQueryable());

        var result = await _authService.RegisterAsync("New", "User", email, "Password123", DateTime.Today.AddYears(-20), "Female");

        result.IsSuccess.ShouldBeTrue();
        await _memberRepo.Received(1).AddAsync(Arg.Is<Member>(m => m.Email == email));
        await _unitOfWork.Received(1).CompleteAsync();
    }
}
