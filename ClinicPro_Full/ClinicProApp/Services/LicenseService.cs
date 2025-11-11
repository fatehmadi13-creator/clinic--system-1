using System;
using System.Security.Cryptography;
using System.Text;

namespace ClinicProApp.Services
{
    public class LicenseService
    {
        private const string Secret = "CHANGE_THIS_SECRET_TO_A_LONG_RANDOM_SECRET_!";

        public bool ValidateLicense(string licenseKey, string clientName, out string message)
        {
            message = "";
            try
            {
                var parts = licenseKey.Split('-');
                if (parts.Length < 3) { message = "Invalid format"; return false; }

                var namePart = parts[0];
                var datePart = parts[1];
                var sigPart = parts[2];

                if (!namePart.Equals(clientName, StringComparison.OrdinalIgnoreCase))
                {
                    message = "Client name mismatch";
                    return false;
                }

                var payload = $"{namePart}-{datePart}";
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var hex = BitConverter.ToString(hash).Replace("-","").Substring(0, 12);

                if (!hex.Equals(sigPart, StringComparison.OrdinalIgnoreCase))
                {
                    message = "Invalid license signature";
                    return false;
                }

                message = "License valid";
                return true;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
