using System;

namespace GymManagementSystem.PL.ViewModels
{
    public class AttendanceQrViewModel
    {
        public int BookingId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string SessionName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string QrCodeBase64 { get; set; } = string.Empty;
    }
}
