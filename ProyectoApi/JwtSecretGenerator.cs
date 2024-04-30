using System;
using System.Security.Cryptography;

public class JwtSecretGenerator
{
    private const string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+";

    public static string GenerateSecret(int length)
    {
        var randomBytes = new byte[length];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
        }

        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            var index = randomBytes[i] % ValidChars.Length;
            chars[i] = ValidChars[index];
        }

        return new string(chars);
    }
}
