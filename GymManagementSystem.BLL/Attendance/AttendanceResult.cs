namespace GymManagementSystem.BLL.Attendance
{
    public enum AttendanceResult
    {
        Success,
        NotFound,
        AlreadyAttended,
        InvalidSignature,
        InvalidFormat,
        SessionNotToday
    }
}
