using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Interfaces;

public interface IEmailService
{
    Task SendOtpAsync(string email, string otp);
}
