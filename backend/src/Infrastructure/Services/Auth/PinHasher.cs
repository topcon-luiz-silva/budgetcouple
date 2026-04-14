namespace BudgetCouple.Infrastructure.Services.Auth;

using BCrypt.Net;

public interface IPinHasher
{
    string HashPin(string pin);
    bool VerifyPin(string pin, string hash);
}

public class PinHasher : IPinHasher
{
    private const int WorkFactor = 12;

    public string HashPin(string pin)
    {
        return BCrypt.HashPassword(pin, WorkFactor);
    }

    public bool VerifyPin(string pin, string hash)
    {
        try
        {
            return BCrypt.Verify(pin, hash);
        }
        catch
        {
            return false;
        }
    }
}
