using System.Security.Cryptography;
using System.Text;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class CustomPasswordHasher : IPasswordHasher
{
    public string HashPassword(string input)
    {
        string hash;
        using SHA256 sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        return hash;
    }
}