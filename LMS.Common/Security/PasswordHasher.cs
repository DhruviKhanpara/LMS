using System.Security.Cryptography;

namespace LMS.Common.Security;

public static class PasswordHasher
{
    #region Create Password Hash
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSolt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSolt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
    #endregion

    #region Verify Password
    public static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSolt)
    {
        using (var hmac = new HMACSHA512(passwordSolt))
        {
            var computedPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedPasswordHash.SequenceEqual(passwordHash);
        }
    }
    #endregion
}
