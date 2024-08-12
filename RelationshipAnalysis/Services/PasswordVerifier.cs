using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class PasswordVerifier : IPasswordVerifier
{
    private readonly IPasswordHasher _passwordHasher;

    public PasswordVerifier(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public bool VerifyPasswordHash(string password, string storedHash)
    {
        return _passwordHasher.HashPassword(password) == storedHash;
    }
}