using System;
using System.Security.Cryptography;
using System.Text;

namespace GymManagementSystem.BLL.Attendance
{
    public static class CheckInPayloadHelper
    {
        private static string ComputeSignature(int bookingId, string secretKey)
        {
            var data = bookingId.ToString();
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string BuildPayload(int bookingId, string secretKey)
        {
            var signature = ComputeSignature(bookingId, secretKey);
            return $"GYMYCHECKIN:{bookingId}:{signature}";
        }

        public static (bool isValidFormat, bool isValidSignature, int bookingId) ParseAndValidate(string payload, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                return (false, false, 0);
            }

            if (!payload.StartsWith("GYMYCHECKIN:"))
            {
                return (false, false, 0);
            }

            var cleanPayload = payload.Substring("GYMYCHECKIN:".Length);
            var parts = cleanPayload.Split(':', 2);
            if (parts.Length != 2)
            {
                return (false, false, 0);
            }

            if (!int.TryParse(parts[0], out var bookingId))
            {
                return (false, false, 0);
            }

            var signature = parts[1];
            var expectedSignature = ComputeSignature(bookingId, secretKey);

            var isValidSignature = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(signature),
                Encoding.UTF8.GetBytes(expectedSignature)
            );

            return (true, isValidSignature, bookingId);
        }
    }
}
