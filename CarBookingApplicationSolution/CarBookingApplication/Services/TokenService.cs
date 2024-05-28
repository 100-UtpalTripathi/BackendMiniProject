using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration configuration)
    {
        _secretKey = configuration?.GetSection("TokenKey")?.GetSection("JWT")?.Value;
        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new ArgumentException("JWT secret key is missing or empty.");
        }
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    }

    public string GenerateToken(Customer customer)
    {
        var claims = new List<Claim>()
        {
            new Claim("eid", customer.Id.ToString()),
            new Claim(ClaimTypes.Role, customer.Role)
        };

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            null,
            null,
            claims,
            expires: DateTime.Now.AddDays(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
