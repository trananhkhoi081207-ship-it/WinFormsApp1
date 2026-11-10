using System.Security.Cryptography;

namespace LabMaintenanceApp;

public static class PasswordHasher
{
    private const int Iterations = 600_000;
    private const string Prefix = "pbkdf2-sha256$";

    public static bool IsHashed(string value) => value.StartsWith(Prefix, StringComparison.Ordinal);

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"{Prefix}{Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        if (!IsHashed(storedHash)) return false;
        string[] parts = storedHash.Split('$');
        if (parts.Length != 4 || !int.TryParse(parts[1], out int iterations)
            || iterations < 100_000 || iterations > 1_000_000) return false;
        try
        {
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expected = Convert.FromBase64String(parts[3]);
            if (salt.Length != 16 || expected.Length != 32) return false;
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
