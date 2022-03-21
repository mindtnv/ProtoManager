using System.Security.Cryptography;
using System.Text;
using ProtoManager.Abstractions;

namespace ProtoManager;

public class PasswordGenerator : IPasswordGenerator
{
    public string Generate() =>
        Base64Encode(Base64Encode(
            RandomNumberGenerator.GetInt32(int.MaxValue / 10, int.MaxValue).ToString()
        ) + "p" + RandomNumberGenerator.GetInt32(-100, 100))[..^2];

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }
}