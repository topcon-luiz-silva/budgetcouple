namespace BudgetCouple.Infrastructure.Services.Auth;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public interface IJwtTokenService
{
    string GenerateToken();
}

public class JwtTokenOptions
{
    public string Issuer { get; set; } = "BudgetCouple";
    public string Audience { get; set; } = "BudgetCouple";
    public string Secret { get; set; } = null!;
    public int ExpiresMinutes { get; set; } = 43200; // 30 days
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtTokenOptions _options;

    public JwtTokenService(IOptions<JwtTokenOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "budgetcouple"),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
