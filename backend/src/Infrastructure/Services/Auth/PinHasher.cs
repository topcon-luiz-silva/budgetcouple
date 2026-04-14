namespace BudgetCouple.Infrastructure.Services.Auth;

using BudgetCouple.Application.Common.Interfaces;
using BCrypt.Net;

public class PinHasher : IPinHasher
{
    private const int WorkFactor = 12;

    public string Hash(string pin)
    {
        return BCrypt.HashPassword(pin, WorkFactor);
    }

    public bool Verify(string pin, string hash)
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
