namespace BudgetCouple.Domain.Identity;

using BudgetCouple.Domain.Common;

/// <summary>
/// Aggregate root for application configuration and PIN management.
/// Shared configuration for the couple's PIN and preferences.
/// </summary>
public class AppConfig : AggregateRoot
{
    public string PinHash { get; private set; } = null!;
    public DateTime? PinSetAt { get; private set; }
    public int FailedAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public string Tema { get; private set; } = "light"; // light, dark
    public string? EmailNotificacao { get; private set; }
    public List<string> TelegramChatIds { get; private set; } = new();

    // EF constructor
    protected AppConfig() { }

    public static AppConfig Create(string pinHash)
    {
        return new AppConfig
        {
            Id = Guid.NewGuid(),
            PinHash = pinHash,
            PinSetAt = DateTime.UtcNow,
            FailedAttempts = 0,
            LockedUntil = null,
            Tema = "light",
            EmailNotificacao = null,
            TelegramChatIds = new()
        };
    }

    /// <summary>
    /// Attempts login with the provided PIN hash.
    /// Returns success if PIN matches and account is not locked.
    /// Increments failed attempts on failure.
    /// </summary>
    public Result TryLogin(string pinHashAttempt, DateTime now)
    {
        // Check if account is locked
        if (LockedUntil.HasValue && LockedUntil > now)
        {
            return Result.Failure(Error.Forbidden("PIN account is locked. Try again later."));
        }

        // Verify PIN matches
        if (PinHash != pinHashAttempt)
        {
            RegistrarFalha(now);
            return Result.Failure(Error.Unauthorized("Invalid PIN."));
        }

        // Reset failed attempts on successful login
        ResetarFalhas();
        return Result.Success();
    }

    /// <summary>
    /// Records a failed login attempt and locks account if threshold reached.
    /// </summary>
    public void RegistrarFalha(DateTime now)
    {
        FailedAttempts++;

        // Lock account after 5 failed attempts for 15 minutes
        if (FailedAttempts >= 5)
        {
            LockedUntil = now.AddMinutes(15);
        }
    }

    /// <summary>
    /// Resets failed attempt counter and removes lock.
    /// </summary>
    public void ResetarFalhas()
    {
        FailedAttempts = 0;
        LockedUntil = null;
    }

    /// <summary>
    /// Changes the PIN to a new hash.
    /// </summary>
    public void TrocarPin(string novoHash)
    {
        PinHash = novoHash;
        PinSetAt = DateTime.UtcNow;
        ResetarFalhas();
    }

    /// <summary>
    /// Updates user preferences.
    /// </summary>
    public void AtualizarPreferencias(string? tema = null, string? emailNotificacao = null)
    {
        if (tema != null)
            Tema = tema;

        if (emailNotificacao != null)
            EmailNotificacao = emailNotificacao;
    }

    public void AdicionarTelegramChatId(string chatId)
    {
        if (!TelegramChatIds.Contains(chatId))
            TelegramChatIds.Add(chatId);
    }

    public void RemoverTelegramChatId(string chatId)
    {
        TelegramChatIds.Remove(chatId);
    }
}
