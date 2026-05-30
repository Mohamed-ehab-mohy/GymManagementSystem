using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Attendance
{
    public interface IAttendanceService
    {
        string BuildCheckInPayload(int bookingId);
        byte[] GenerateQrImage(string payload);
        Task<(AttendanceResult result, string? memberName, string? sessionName)> CheckInAsync(string payload);
    }
}
