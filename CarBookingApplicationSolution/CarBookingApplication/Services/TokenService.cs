using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    #region Fields

    private readonly string _secretKey;
    private readonly SymmetricSecurityKey _key;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration containing the secret key.</param>
    /// <exception cref="ArgumentException">Thrown when JWT secret key is missing or empty.</exception>
    public TokenService(IConfiguration configuration)
    {
        _secretKey = configuration?.GetSection("TokenKey")?.GetSection("JWT")?.Value;
        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new ArgumentException("JWT secret key is missing or empty.");
        }
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    }

    #endregion

    #region Generate-Token
    /// <summary>
    /// Generates a JWT token for the specified customer.
    /// </summary>
    /// <param name="customer">The customer for whom the token is generated.</param>
    /// <returns>The generated JWT token.</returns>

    public string GenerateToken(Customer customer)
    {
        var claims = new List<Claim>()
        {
            new Claim("eid", customer.Id.ToString()),
            new Claim("role", customer.Role),
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

    #endregion
}
