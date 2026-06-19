using System.Security.Claims;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;

namespace GymManagementSystem.BLL.Interfaces;

public interface IAuthService
{
    Task<Result<ClaimsPrincipal>> LoginAsync(string email, string password);
    Task<Result> RegisterAsync(string firstName, string lastName, string email, string password, DateTime dateOfBirth, string gender);
    Task<Result> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<Result> RegisterAdminAsync(string email, string password, string role);
    Task<GymUser?> FindByEmailAsync(string email);
    Task<ClaimsPrincipal> SignInUserAsync(GymUser user);
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(string email, string otp, string newPassword);
}
