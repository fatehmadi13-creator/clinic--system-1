using System;
using System.Security.Cryptography;
using System.Text;

class LicenseGenerator
{
    private const string Secret = "CHANGE_THIS_SECRET_TO_A_LONG_RANDOM_SECRET_!";

    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: LicenseGenerator <ClientName> <YYYYMMDD>");
            return;
        }

        var clientName = args[0].ToUpper().Replace(" ", "");
        var date = args[1];
        var payload = $"{clientName}-{date}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var hex = BitConverter.ToString(hash).Replace("-", "").Substring(0, 12);
        var license = $"{clientName}-{date}-{hex}";
        Console.WriteLine($"Generated License: {license}");
    }
}
