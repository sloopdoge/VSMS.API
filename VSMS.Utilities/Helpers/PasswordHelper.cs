namespace VSMS.Utilities.Helpers;

public static class PasswordHelper
{
    private static readonly char[] Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] Lowercase = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly char[] Digits = "0123456789".ToCharArray();
    private static readonly char[] Special = "!@#$%^&*()_+-=[]{}|;:,.<>?".ToCharArray();
    private static readonly char[] All = Uppercase.Concat(Lowercase).Concat(Digits).Concat(Special).ToArray();

    private static readonly Random Random = new();

    /// <summary>
    /// Generates a secure random password of specified length that includes:
    /// at least one uppercase letter, one digit, and one special character.
    /// The remaining characters are randomly selected from uppercase, lowercase,
    /// digits, and special characters. The result is shuffled to ensure randomness.
    /// </summary>
    /// <param name="length">The total length of the password. Must be at least 4.</param>
    /// <returns>A randomly generated password string that satisfies complexity requirements.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified length is less than 4.</exception>
    public static string GeneratePassword(int length = 10)
    {
        if (length < 4)
            throw new ArgumentException("Password length must be at least 4 characters.");

        var passwordChars = new List<char>
        {
            Uppercase[Random.Next(Uppercase.Length)],   // Ensure 1 capital
            Digits[Random.Next(Digits.Length)],         // Ensure 1 digit
            Special[Random.Next(Special.Length)],       // Ensure 1 special
            Lowercase[Random.Next(Lowercase.Length)]    // Ensure some lowercase
        };

        // Fill the rest with the password
        for (int i = passwordChars.Count; i < length; i++)
        {
            passwordChars.Add(All[Random.Next(All.Length)]);
        }

        // Shuffle to avoid predictable positions
        return new string(passwordChars.OrderBy(_ => Random.Next()).ToArray());
    }
}

